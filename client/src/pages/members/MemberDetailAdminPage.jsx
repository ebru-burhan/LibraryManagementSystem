import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { memberService } from '../../services/api';
import { PERMISSIONS } from '../../auth/permissionKeys';
import { useAuth } from '../../hooks/useAuth';
import './MemberDetailAdminPage.css';

const FILE_BASE = 'https://localhost:7213';

function formatDate(value) {
  if (!value) return '—';
  return new Date(value).toLocaleDateString('tr-TR');
}

function formatMoney(amount) {
  return `${Number(amount || 0).toFixed(2)} ₺`;
}

export default function MemberDetailAdminPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { permissions } = useAuth();
  const canManage = permissions.includes(PERMISSIONS.MANAGE_MEMBERS);

  const [member, setMember] = useState(null);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ text: '', type: '' });

  const fetchMember = async () => {
    try {
      setLoading(true);
      const response = await memberService.getById(id);
      if (response.success) {
        setMember(response.data);
      }
    } catch (error) {
      setMessage({ text: error.response?.data?.message || 'Üye kartı yüklenemedi.', type: 'error' });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchMember();
  }, [id]);

  const handleStatusChange = async (statusCode) => {
    try {
      const response = await memberService.updateStatus(id, statusCode);
      if (response.success) {
        setMessage({ text: response.message, type: 'success' });
        fetchMember();
      }
    } catch (error) {
      setMessage({ text: error.response?.data?.message || 'Durum güncellenemedi.', type: 'error' });
    }
  };

  const handleDelete = async () => {
    const confirmed = window.confirm(`${member.fullName} adlı üye kaydı silinsin mi?`);
    if (!confirmed) return;

    try {
      const response = await memberService.remove(id);
      if (response.success) {
        navigate('/members');
      }
    } catch (error) {
      setMessage({ text: error.response?.data?.message || 'Üye silinemedi.', type: 'error' });
    }
  };

  if (loading) {
    return <div className="member-detail-page">Üye kartı yükleniyor...</div>;
  }

  if (!member) {
    return <div className="member-detail-page">Üye bulunamadı.</div>;
  }

  const statusClass =
    member.status === 'ACTIVE' ? 'badge-active' : member.status === 'PASSIVE' ? 'badge-passive' : 'badge-suspended';

  return (
    <div className="member-detail-page">
      <div className="member-breadcrumb">
        <Link to="/members">Üyeler</Link>
        <span>/</span>
        <strong>Üye Kartı</strong>
      </div>

      {message.text && <div className={`admin-alert ${message.type}`}>{message.text}</div>}
<section className="member-profile-card">
        {member.pictureUrl ? (
          <img src={`${FILE_BASE}${member.pictureUrl}`} alt={member.fullName} />
        ) : (
          <div className="member-profile-fallback">{member.fullName?.slice(0, 1)}</div>
        )}
        
        <div className="member-profile-info">
          
          {/* Üst Kısım: İsim, ID ve Sağdaki Butonlar */}
          <div className="profile-top-row">
            <div>
              <div className="member-profile-title">
                <h2>{member.fullName}</h2>
                <span className={`member-detail-badge ${statusClass}`}>{member.statusName} Üye</span>
              </div>
              <p className="profile-id-text">
                ID: {member.memberNumber}{member.membershipType ? ` • ${member.membershipType}` : ''}
              </p>
            </div>

            {/* Sağ Tarafa Yaslı Aksiyon Butonları */}
            {canManage && (
              <div className="profile-actions-inline">
                {member.status !== 'ACTIVE' && (
                  <button type="button" className="btn-activate" onClick={() => handleStatusChange('ACTIVE')}>
                    Aktife Al
                  </button>
                )}
                {member.status !== 'PASSIVE' && (
                  <button type="button" className="btn-passive" onClick={() => handleStatusChange('PASSIVE')}>
                    Pasife Al
                  </button>
                )}
                <button type="button" className="btn-delete" onClick={handleDelete}>
                  Üyeyi Sil
                </button>
              </div>
            )}
          </div>

          {/* Ortadaki Ayırıcı Çizgi */}
          <hr className="profile-divider" />

          {/* Alt Kısım: Yan Yana İletişim Bilgileri */}
          <div className="profile-meta-row">
            <div className="meta-item">
              <span>E-posta</span>
              <strong>{member.email}</strong>
            </div>
            <div className="meta-item">
              <span>Telefon</span>
              <strong>{member.phone || '—'}</strong>
            </div>
            <div className="meta-item">
              <span>Kayıt Tarihi</span>
              <strong>{formatDate(member.registrationDate)}</strong>
            </div>
            <div className="meta-item">
              <span>Adres</span>
              <strong>{member.address || '—'}</strong>
            </div>
          </div>
          
        </div>
      </section>

      <section className="member-section-card">
        <h3>Aktif Ödünçler</h3>
        {member.activeLoans?.length ? (
          <table className="admin-table">
            <thead>
              <tr>
                <th>Kitap Adı</th>
                <th>Ödünç Tarihi</th>
                <th>İade Tarihi</th>
                <th>Durum</th>
              </tr>
            </thead>
            <tbody>
              {member.activeLoans.map((loan) => (
                <tr key={loan.id}>
                  <td>
                    <strong>{loan.bookTitle}</strong>
                    {loan.authors && <small> {loan.authors}</small>}
                  </td>
                  <td>{formatDate(loan.loanDate)}</td>
                  <td className={loan.isOverdue ? 'overdue-date' : ''}>{formatDate(loan.dueDate)}</td>
                  <td>
                    <span className={loan.isOverdue ? 'loan-badge overdue' : 'loan-badge'}>
                      {loan.isOverdue ? 'Gecikmiş' : 'Zamanında'}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p className="empty-note">Aktif ödünç bulunmamaktadır.</p>
        )}
      </section>

      <div className="member-detail-split">
        <section className="member-section-card">
          <h3>Cezalar ve İşlemler</h3>
          {member.penalties?.length ? (
            <table className="admin-table">
              <thead>
                <tr>
                  <th>Neden</th>
                  <th>Oluşturulma</th>
                  <th>Gecikme</th>
                  <th>Tutar</th>
                  <th>Durum</th>
                </tr>
              </thead>
              <tbody>
                {member.penalties.map((penalty) => (
                  <tr key={penalty.id}>
                    <td>
                      {penalty.reason}
                      {penalty.relatedBookTitle ? ` (${penalty.relatedBookTitle})` : ''}
                    </td>
                    <td>{formatDate(penalty.createdAt)}</td>
                    <td>{penalty.delayDays} Gün</td>
                    <td className={!penalty.isPaid ? 'overdue-date' : ''}>{formatMoney(penalty.amount)}</td>
                    <td>
                      <span className={penalty.isPaid ? 'loan-badge' : 'loan-badge overdue'}>
                        {penalty.isPaid ? 'Ödendi' : 'Ödenmedi'}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <p className="empty-note">Kayıtlı ceza bulunmamaktadır.</p>
          )}
        </section>

        <section className="member-section-card">
          <h3>Rezervasyonlar</h3>
          {member.reservations?.length ? (
            <ul className="reservation-list">
              {member.reservations.map((reservation) => (
                <li key={reservation.id}>
                  <strong>{reservation.bookTitle}</strong>
                  {reservation.authors && <span> — {reservation.authors}</span>}
                  <small>Sıra: {reservation.queueNumber}</small>
                </li>
              ))}
            </ul>
          ) : (
            <p className="empty-note">Başka rezervasyon bulunmamaktadır.</p>
          )}
        </section>
      </div>
    </div>
  );
}
