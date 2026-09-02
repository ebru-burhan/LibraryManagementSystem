import React, { useState, useEffect } from 'react';
import { membershipService } from '../../services/api';
import './MembershipApplicationListAdminPage.css';

export default function MembershipApplicationListAdminPage() {
  const [applications, setApplications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ text: '', type: '' });
  const [selectedApp, setSelectedApp] = useState(null);

  const fetchApplications = async () => {
    try {
      setLoading(true);
      const response = await membershipService.getAllApplications();
      if (response.success) {
        setApplications(response.data);
      }
    } catch (error) {
      console.error("Başvurular yüklenirken hata oluştu:", error);
      setMessage({ text: 'Başvurular getirilemedi.', type: 'error' });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchApplications();
  }, []);

  const handleApprove = async (id) => {
    try {
      const response = await membershipService.approveApplication(id);
      if (response.success) {
        setMessage({ text: response.message, type: 'success' });
        setSelectedApp(null);
        fetchApplications();
      }
    } catch (error) {
      setMessage({ text: error.response?.data?.message || 'Onaylama başarısız.', type: 'error' });
    }
  };

  const handleReject = async (id) => {
    try {
      const response = await membershipService.rejectApplication(id);
      if (response.success) {
        setMessage({ text: response.message, type: 'success' });
        setSelectedApp(null);
        fetchApplications();
      }
    } catch (error) {
      setMessage({ text: error.response?.data?.message || 'Reddetme başarısız.', type: 'error' });
    }
  };

  const handleDownload = async (fileUrl) => {
    try {
      const fullUrl = `https://localhost:7213${fileUrl}`;
      const response = await fetch(fullUrl);
      if (!response.ok) throw new Error(`Sunucu hatası: ${response.status}`);
      
      const blob = await response.blob();
      const blobUrl = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = blobUrl;
      
      const fileName = fileUrl.split('/').pop() || 'belge.pdf';
      link.download = fileName;
      
      document.body.appendChild(link);
      link.click();
      
      document.body.removeChild(link);
      window.URL.revokeObjectURL(blobUrl);
    } catch (error) {
      console.error("Detaylı indirme hatası:", error);
      alert("Dosya indirilemedi.");
    }
  };

  if (loading) return <div className="admin-applications-container">Yükleniyor...</div>;

  return (
    <div className="admin-applications-container">
      {selectedApp ? (
        <div className="detail-view-wrapper">
          <div className="detail-header-nav">
            <span onClick={() => setSelectedApp(null)} className="back-link">
              Applications
            </span> 
            <span className="separator">/</span> 
            <span className="current-id">APP-{selectedApp.id}</span>
          </div>

          <div className="detail-top-bar">
            <div>
              <h2 className="admin-applications-title">Başvuru Detayı</h2>
              <p className="detail-subtitle">Review applicant information and process membership request.</p>
            </div>
          </div>

          {/* İki Sütunlu Ana Yapı */}
          <div className="detail-grid-layout">
            
            {/* SOL TARAF: Bilgiler ve Belgeler */}
            <div className="detail-left-column">
              <div className="applicant-card">
                {selectedApp.pictureUrl && (
                  <img 
                    src={`https://localhost:7213${selectedApp.pictureUrl}`} 
                    alt="Profil" 
                    className="applicant-avatar" 
                  />
                )}
                <div className="applicant-info-area">
                  <h3>{selectedApp.firstName} {selectedApp.lastName}</h3>
                  <p className="sub-info">{selectedApp.membershipTypeName || 'Standard Member'} • ID: {selectedApp.identityNumber || 'N/A'}</p>
                  
                  <div className="info-grid-row">
                    <div>
                      <span className="info-label">Email Address</span>
                      <p className="info-val">{selectedApp.email}</p>
                    </div>
                    <div>
                      <span className="info-label">Phone Number</span>
                      <p className="info-val">{selectedApp.phoneNumber || 'N/A'}</p>
                    </div>
                  </div>

                  <div className="info-grid-row" style={{ marginTop: '1rem' }}>
                    <div>
                      <span className="info-label">Application Date</span>
                      <p className="info-val">{new Date(selectedApp.createdAt).toLocaleDateString('tr-TR')}</p>
                    </div>
                    <div>
                      <span className="info-label">Address</span>
                      <p className="info-val">{selectedApp.address || 'Belirtilmemiş'}</p>
                    </div>
                  </div>
                </div>
              </div>

              {/* Belgeler Bölümü */}
              <div className="documents-section">
                <h4>Provided Documents</h4>
                
                <div className="doc-item-card">
                  <div className="doc-icon">📄</div>
                  <div className="doc-details">
                    <h5>Identity Verification / Supporting Document</h5>
                    <p>PDF • Official Document</p>
                  </div>
                  {selectedApp.documentUrl ? (
                    <div style={{ display: 'flex', gap: '10px', marginLeft: 'auto' }}>
                      <a 
                        href={`https://localhost:7213${selectedApp.documentUrl}`} 
                        target="_blank" 
                        rel="noopener noreferrer"
                        className="doc-action-btn"
                        style={{ textDecoration: 'none' }}
                      >
                        View
                      </a>
                      <button 
                        onClick={() => handleDownload(selectedApp.documentUrl)}
                        className="doc-action-btn"
                        style={{ background: 'none', border: 'none', cursor: 'pointer', padding: 0, color: 'var(--primary-color)', fontWeight: 600, textDecoration: 'underline' }}
                      >
                        Download
                      </button>
                    </div>
                  ) : (
                    <span className="no-doc-text" style={{ marginLeft: 'auto' }}>Yüklenmemiş</span>
                  )}
                </div>
              </div>
            </div>

            {/* SAĞ TARAF: Yönetim Paneli (Admin Action Card) */}
            <div className="detail-right-column">
              <div className="action-panel-card">
                <h3>Administration Panel</h3>
                <p className="action-panel-desc">Applicant will receive a notification email upon decision.</p>

                {/* DURUM ROZETİ BURAYA EKLENDİ */}
                <div style={{ marginBottom: '1.25rem', textAlign: 'center' }}>
                  <span className={`status-badge-pill ${
                    selectedApp.applicationStatus === 'PENDING' ? 'badge-pending' : 
                    selectedApp.applicationStatus === 'APPROVED' ? 'badge-approved' : 'badge-rejected'
                  }`} style={{ display: 'inline-block', width: '100%', padding: '0.6rem', textAlign: 'center', borderRadius: '8px' }}>
                    {selectedApp.applicationStatus === 'PENDING' ? 'Pending Review' : selectedApp.applicationStatus}
                  </span>
                </div>

                {selectedApp.applicationStatus?.toUpperCase() === 'PENDING' && (
                  <div className="action-buttons-stack">
                    <button 
                      onClick={() => handleApprove(selectedApp.id)}
                      className="btn-action-approve"
                    >
                      ✓ Approve Membership
                    </button>

                    <div className="divider-or"><span>or</span></div>

                    <button 
                      onClick={() => handleReject(selectedApp.id)}
                      className="btn-action-reject"
                    >
                      ✕ Reject Application
                    </button>
                  </div>
                )}

                <button 
                  onClick={() => setSelectedApp(null)}
                  className="btn-back-to-list"
                >
                  ← Back to List
                </button>
              </div>
            </div>

          </div>
        </div>
      ) : (
        /* LİSTE GÖRÜNÜMÜ */
        <>
          <h2 className="admin-applications-title">Üyelik Başvuruları</h2>

          {message.text && (
            <div className={`admin-alert ${message.type}`}>
              {message.text}
            </div>
          )}

          <div className="table-responsive">
            <table className="admin-table">
              <thead>
                <tr>
                  <th>Ad Soyad</th>
                  <th>E-Posta</th>
                  <th>Başvuru Tarihi</th>
                  <th>Durum</th>
                  <th className="th-center">İşlemler</th>
                </tr>
              </thead>
              <tbody>
                {applications.length > 0 ? (
                  applications.map((app) => (
                    <tr key={app.id}>
                      <td>{app.firstName} {app.lastName}</td>
                      <td>{app.email}</td>
                      <td>{new Date(app.createdAt).toLocaleString('tr-TR')}</td>
                      <td>
                        <span className={`badge ${
                          app.applicationStatus === 'PENDING' ? 'badge-pending' : 
                          app.applicationStatus === 'APPROVED' ? 'badge-approved' : 'badge-rejected'
                        }`}>
                          {app.applicationStatus}
                        </span>
                      </td>
                      <td className="td-center">
                        <button 
                            onClick={() => setSelectedApp(app)}
                            className="btn-view-detail"
                        >
                            View Detail
                        </button>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan="5" className="td-empty">
                      Bekleyen başvuru bulunmamaktadır.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  );
}