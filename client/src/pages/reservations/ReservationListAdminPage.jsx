import React, { useState, useEffect } from 'react';
import { reservationService } from '../../services/api';
import './ReservationListAdminPage.css';

export default function ReservationListAdminPage() {
  const [reservations, setReservations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ text: '', type: '' });

  useEffect(() => {
    fetchReservations();
  }, []);

  const fetchReservations = async () => {
    try {
      setLoading(true);
      const response = await reservationService.getAll();
      if (response.success) {
        setReservations(response.data);
      }
    } catch (error) {
      console.error("Rezervasyonlar çekilirken hata:", error);
      setMessage({ text: 'Rezervasyon kuyruğu getirilemedi.', type: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const totalReservations = reservations.length;
  const waitingCount = reservations.filter(r => r.status === 'WAITING').length;
  const readyCount = reservations.filter(r => r.status === 'READY').length;

  if (loading) return <div className="admin-page-container">Kuyruk yükleniyor...</div>;

  return (
    <div className="admin-page-container">
      <h2>Rezervasyon Yönetimi</h2>
      <p className="page-subtitle">Bekleyen kitap rezervasyonlarını ve sıra numaralarını takip edin.</p>

      {message.text && (
        <div className={`admin-alert ${message.type}`}>
          {message.text}
        </div>
      )}

      {/* KPI Kartları */}
      <div className="kpi-cards-row">
        <div className="kpi-card">
          <h4 className="kpi-title">TOPLAM REZERVASYON</h4>
          <span className="kpi-value orange">{totalReservations}</span>
        </div>
        <div className="kpi-card">
          <h4 className="kpi-title">BEKLEYENLER</h4>
          <span className="kpi-value dark">{waitingCount}</span>
        </div>
        <div className="kpi-card">
          <h4 className="kpi-title">TESLİMAT BEKLEYEN (HAZIR)</h4>
          <span className="kpi-value green">{readyCount}</span>
        </div>
      </div>

      {/* Kuyruk Tablosu */}
      <div className="table-container">
        <table className="admin-table">
          <thead>
            <tr>
              <th>Üye</th>
              <th>Kitap</th>
              <th>Rezervasyon Tarihi</th>
              <th style={{ textAlign: 'center' }}>Sıra Numarası</th>
              <th>Durum</th>
            </tr>
          </thead>
          <tbody>
            {reservations.length > 0 ? (
              reservations.map((res) => (
                <tr key={res.id}>
                  <td>
                    <strong>{res.memberFullName}</strong>
                    <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)' }}>{res.memberEmail}</div>
                  </td>
                  <td>
                    <strong>{res.bookTitle}</strong>
                  </td>
                  <td>{new Date(res.reservationDate).toLocaleDateString('tr-TR')}</td>
                  <td style={{ textAlign: 'center' }}>
                    <span className="queue-number-badge">
                      {res.queueNumber}
                    </span>
                  </td>
                  <td>
                    <span className={`badge ${res.status === 'READY' ? 'badge-active' : 'badge-pending'}`}>
                      {res.status === 'READY' ? 'Hazır' : 'Sırada Bekliyor'}
                    </span>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="5" className="empty-row-text">
                  Aktif rezervasyon kaydı bulunmamaktadır.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}