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

  if (loading) return <div className="admin-applications-container">Yükleniyor...</div>;

  return (
    <div className="admin-applications-container">
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
              <th style={{ textAlign: 'center' }}>İşlemler</th>
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
                      app.applicationStatus === 'APPROVED' ? 'badge-approved' : 
                      'badge-rejected'
                    }`}>
                      {app.applicationStatus}
                    </span>
                  </td>
                  <td style={{ textAlign: 'center' }}>
                    <button 
                        onClick={() => {
                            console.log("Seçilen Başvuru Durumu:", app.applicationStatus);
                            setSelectedApp(app);
                        }}
                        className="btn-view-detail"
                        >
                        View Detail
                    </button>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="4" style={{ textAlign: 'center', color: '#6b7280' }}>
                  Bekleyen başvuru bulunmamaktadır.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {selectedApp && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h3>Başvuru Detayı</h3>
            <div className="modal-body">
              <p><strong>Ad Soyad:</strong> {selectedApp.firstName} {selectedApp.lastName}</p>
              <p><strong>E-Posta:</strong> {selectedApp.email}</p>
              <p><strong>Başvuru Tarihi:</strong> {new Date(selectedApp.createdAt).toLocaleString('tr-TR')}</p>
              <p><strong>Telefon:</strong> {selectedApp.phoneNumber || 'Belirtilmemiş'}</p>
              <p><strong>Adres:</strong> {selectedApp.address || 'Belirtilmemiş'}</p>
              <p><strong>Durum:</strong> {selectedApp.applicationStatus}</p>
            </div>

           <div className="modal-footer">
              <div className="modal-actions">
                {selectedApp.applicationStatus?.toUpperCase() === 'PENDING' && (
                  <>
                    <button 
                      onClick={() => handleApprove(selectedApp.id)}
                      className="btn-approve"
                    >
                      Onayla
                    </button>
                    <button 
                      onClick={() => handleReject(selectedApp.id)}
                      className="btn-reject"
                    >
                      Reddet
                    </button>
                  </>
                )}
              </div>
              <button 
                onClick={() => setSelectedApp(null)}
                className="btn-close"
              >
                Kapat
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}