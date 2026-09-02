import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { memberService } from '../../services/api';
import { PERMISSIONS } from '../../auth/permissionKeys';
import { useAuth } from '../../hooks/useAuth';
import './MemberListAdminPage.css';

const FILE_BASE = 'https://localhost:7213';

const STATUS_FILTERS = [
  { key: 'ALL', label: 'Tümü' },
  { key: 'ACTIVE', label: 'Aktif' },
  { key: 'PASSIVE', label: 'Pasif' },
  { key: 'SUSPENDED', label: 'Askıya Alınmış' },
];

function statusClass(status) {
  if (status === 'ACTIVE') return 'member-status-active';
  if (status === 'PASSIVE') return 'member-status-passive';
  return 'member-status-suspended';
}

function initials(fullName) {
  return (fullName || '')
    .split(' ')
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0])
    .join('')
    .toUpperCase();
}

export default function MemberListAdminPage() {
  const navigate = useNavigate();
  const { permissions } = useAuth();
  const canManage = permissions.includes(PERMISSIONS.MANAGE_MEMBERS);

  const [directory, setDirectory] = useState({
    members: [],
    totalCount: 0,
    activeCount: 0,
    passiveCount: 0,
    suspendedCount: 0,
  });
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ text: '', type: '' });
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('ALL');
  const [openMenuId, setOpenMenuId] = useState(null);

  const fetchMembers = async () => {
    try {
      setLoading(true);
      const response = await memberService.getAll();
      if (response.success) {
        setDirectory(response.data);
      }
    } catch (error) {
      console.error(error);
      setMessage({ text: 'Üyeler getirilemedi.', type: 'error' });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchMembers();
  }, []);

  const filteredMembers = useMemo(() => {
    const term = search.trim().toLowerCase();
    return (directory.members || []).filter((member) => {
      const matchesStatus = statusFilter === 'ALL' || member.status === statusFilter;
      const haystack = `${member.fullName} ${member.email} ${member.memberNumber} ${member.phone || ''}`.toLowerCase();
      const matchesSearch = !term || haystack.includes(term);
      return matchesStatus && matchesSearch;
    });
  }, [directory.members, search, statusFilter]);

  const handleStatusChange = async (id, statusCode) => {
    try {
      const response = await memberService.updateStatus(id, statusCode);
      if (response.success) {
        setMessage({ text: response.message, type: 'success' });
        setOpenMenuId(null);
        fetchMembers();
      }
    } catch (error) {
      setMessage({ text: error.response?.data?.message || 'Durum güncellenemedi.', type: 'error' });
    }
  };

  const handleDelete = async (member) => {
    const confirmed = window.confirm(`${member.fullName} adlı üye kaydı silinsin mi?`);
    if (!confirmed) return;

    try {
      const response = await memberService.remove(member.id);
      if (response.success) {
        setMessage({ text: response.message, type: 'success' });
        setOpenMenuId(null);
        fetchMembers();
      }
    } catch (error) {
      setMessage({ text: error.response?.data?.message || 'Üye silinemedi.', type: 'error' });
    }
  };

  if (loading) {
    return <div className="member-directory-page">Üyeler yükleniyor...</div>;
  }

  const attentionCount = (directory.passiveCount || 0) + (directory.suspendedCount || 0);

  return (
    <div className="member-directory-page" onClick={() => setOpenMenuId(null)}>
      <div className="member-directory-header">
        <div>
          <h2>Üye Dizini</h2>
          <p>Kütüphane üyelerini yönetin, durumlarını görüntüleyin ve iletişim bilgilerini güncelleyin.</p>
        </div>
      </div>

      {message.text && (
        <div className={`admin-alert ${message.type}`}>{message.text}</div>
      )}

      <div className="member-stat-grid">
        <article className="member-stat-card">
          <span>Toplam Üye</span>
          <strong>{directory.totalCount}</strong>
        </article>
        <article className="member-stat-card">
          <span>Aktif Durum</span>
          <strong>{directory.activeCount}</strong>
        </article>
        <article className="member-stat-card attention">
          <span>Dikkat Gerektiren</span>
          <strong>{attentionCount}</strong>
          <small>askıya alınmış / pasif</small>
        </article>
      </div>

      <div className="member-table-toolbar">
        <input
          type="text"
          placeholder="Ada, e-posta veya üye numarasına göre ara..."
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />
        <div className="member-status-tabs">
          {STATUS_FILTERS.map((filter) => (
            <button
              key={filter.key}
              type="button"
              className={statusFilter === filter.key ? 'is-active' : ''}
              onClick={() => setStatusFilter(filter.key)}
            >
              {filter.label}
            </button>
          ))}
        </div>
      </div>

      <div className="table-responsive">
        <table className="admin-table">
          <thead>
            <tr>
              <th>Üye No</th>
              <th>Ad Soyad</th>
              <th>Telefon</th>
              <th>E-posta</th>
              <th>Durum</th>
              <th className="th-center">İşlemler</th>
            </tr>
          </thead>
          <tbody>
            {filteredMembers.length > 0 ? (
              filteredMembers.map((member) => (
                <tr key={member.id}>
                  <td>{member.memberNumber}</td>
                  <td>
                    <div className="member-name-cell">
                      {member.pictureUrl ? (
                        <img src={`${FILE_BASE}${member.pictureUrl}`} alt={member.fullName} />
                      ) : (
                        <span className="member-avatar-fallback">{initials(member.fullName)}</span>
                      )}
                      <div>
                        <strong>{member.fullName}</strong>
                        {member.unpaidDebtAmount > 0 && (
                          <small className="member-debt-hint">Borç: {member.unpaidDebtAmount.toFixed(2)} ₺</small>
                        )}
                      </div>
                    </div>
                  </td>
                  <td>{member.phone || '—'}</td>
                  <td>{member.email}</td>
                  <td>
                    <span className={`member-status-dot ${statusClass(member.status)}`}>
                      {member.statusName}
                    </span>
                  </td>
                  <td className="td-center">
                    <div className="member-actions" onClick={(event) => event.stopPropagation()}>
                      <button
                        type="button"
                        className="member-actions-trigger"
                        onClick={() => setOpenMenuId(openMenuId === member.id ? null : member.id)}
                        aria-label="İşlemler"
                      >
                        ⋮
                      </button>
                      {openMenuId === member.id && (
                        <div className="member-actions-menu">
                          <button type="button" onClick={() => navigate(`/members/${member.id}`)}>
                            Detayı Gör
                          </button>
                          {canManage && member.status !== 'ACTIVE' && (
                            <button type="button" onClick={() => handleStatusChange(member.id, 'ACTIVE')}>
                              Aktife Al
                            </button>
                          )}
                          {canManage && member.status !== 'PASSIVE' && (
                            <button type="button" onClick={() => handleStatusChange(member.id, 'PASSIVE')}>
                              Pasife Al
                            </button>
                          )}
                          {canManage && (
                            <button type="button" className="danger" onClick={() => handleDelete(member)}>
                              Sil
                            </button>
                          )}
                        </div>
                      )}
                    </div>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="6" className="td-empty">Kayıtlı üye bulunamadı.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
