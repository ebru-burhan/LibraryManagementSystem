import React, { useState, useEffect } from 'react';
import { myProfileService, reservationService } from '../../services/api';
import './MyReservationsPage.css';

function formatDate(dateString) {
  if (!dateString) return '—';
  const options = { year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' };
  return new Date(dateString).toLocaleDateString('tr-TR', options);
}

// Backend'den gelen statüye göre CSS sınıfı ve İkon belirleme
function getStatusConfig(status) {
  const s = (status || '').toLowerCase();
  if (s.includes('bekliyor') || s.includes('waiting')) {
    return { class: 'badge-waiting', icon: '⏳', text: 'Sırada Bekliyor' };
  }
  if (s.includes('hazır') || s.includes('completed') || s.includes('ready')) {
    return { class: 'badge-ready', icon: '✅', text: 'Teslim Alınabilir' };
  }
  if (s.includes('iptal') || s.includes('cancelled')) {
    return { class: 'badge-cancelled', icon: '❌', text: 'İptal Edildi' };
  }
  return { class: 'badge-default', icon: '📌', text: status || 'Bilinmiyor' };
}

export default function MyReservationsPage() {
  const [reservations, setReservations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchReservations();
  }, []);

  const fetchReservations = async () => {
    try {
      setLoading(true);
      const response = await myProfileService.getMyReservations();
      if (response.success) {
        setReservations(response.data);
      }
    } catch (err) {
      setError("Rezervasyon bilgileriniz yüklenirken bir hata oluştu.");
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = async (id, bookTitle) => {
    const confirmed = window.confirm(`"${bookTitle}" için rezervasyonunuzu iptal etmek istediğinize emin misiniz?`);
    if (!confirmed) return;

    try {
      const response = await reservationService.cancel(id);
      if (response.success) {
        // Listeyi yenile (İptal edilenleri göstermemek veya statüsünü iptal olarak göstermek için)
        fetchReservations();
      }
    } catch (err) {
      alert(err.response?.data?.message || "Rezervasyon iptal edilemedi.");
    }
  };

  if (loading) {
    return <div className="my-reservations-container"><div className="loading-text">Kuyruk durumunuz kontrol ediliyor...</div></div>;
  }

  return (
    <div className="my-reservations-container">
      <div className="page-header">
        <div>
          <h2>Rezervasyonlarım</h2>
          <p>Kütüphanede müsait kopyası bulunmadığı için sıraya girdiğiniz kitapları ve kuyruk durumunuzu buradan takip edebilirsiniz.</p>
        </div>
      </div>

      {error && <div className="alert-error">{error}</div>}

      {reservations.length > 0 ? (
        <div className="reservations-grid">
          {reservations.map((res) => {
            const statusConfig = getStatusConfig(res.status);
            const isWaiting = statusConfig.text === 'Sırada Bekliyor';
            
            return (
              <div key={res.id} className={`reservation-card ${!isWaiting ? 'is-inactive' : ''}`}>
                <div className="reservation-header">
                  <div className="book-info">
                    <h3>{res.bookTitle}</h3>
                    {res.authors && <span className="book-author">{res.authors}</span>}
                  </div>
                  <span className={`status-badge ${statusConfig.class}`}>
                    {statusConfig.icon} {statusConfig.text}
                  </span>
                </div>

                <div className="reservation-body">
                  <div className="queue-display">
                    <span className="queue-label">Sıra Numaranız</span>
                    <div className={`queue-number ${res.queueNumber === 1 ? 'is-first' : ''}`}>
                      {res.queueNumber}
                    </div>
                    {res.queueNumber === 1 && isWaiting && (
                      <span className="queue-hint">Sıradaki ilk kişi sizsiniz!</span>
                    )}
                  </div>

                  <div className="reservation-details">
                    <div className="detail-row">
                      <span className="detail-label">İşlem Tarihi:</span>
                      <span className="detail-value">{formatDate(res.reservationDate)}</span>
                    </div>
                  </div>
                </div>

                <div className="reservation-footer">
                  {isWaiting ? (
                    <button 
                      className="btn-cancel" 
                      onClick={() => handleCancel(res.id, res.bookTitle)}
                    >
                      Sıradan Çık
                    </button>
                  ) : (
                    <span className="footer-note">Bu rezervasyon artık aktif değildir.</span>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      ) : (
        <div className="empty-state-card">
          <div className="empty-icon">⏳</div>
          <h3>Bekleyen Rezervasyonunuz Yok</h3>
          <p>Şu anda sırada beklediğiniz bir kitap bulunmuyor. Katalog üzerinden müsait olmayan kitaplar için sıraya girebilirsiniz.</p>
        </div>
      )}
    </div>
  );
}