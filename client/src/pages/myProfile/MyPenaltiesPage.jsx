import React, { useState, useEffect } from 'react';
import { myProfileService } from '../../services/api';
import './MyPenaltiesPage.css';

function formatDate(dateString) {
  if (!dateString) return '—';
  return new Date(dateString).toLocaleDateString('tr-TR');
}

function formatMoney(amount) {
  return `${Number(amount || 0).toFixed(2)} ₺`;
}

// Ceza tipine göre ikon belirleme
function getPenaltyIcon(penaltyType) {
  const type = (penaltyType || '').toLowerCase();
  if (type.includes('gecikme') || type.includes('overdue')) return '⏳';
  if (type.includes('hasar') || type.includes('damage')) return '🛠️';
  if (type.includes('kayıp') || type.includes('lost')) return '🔍';
  return '⚠️';
}

export default function MyPenaltiesPage() {
  const [penalties, setPenalties] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Toplam ödenmemiş borcu hesaplamak için
  const totalDebt = penalties
    .filter(p => !p.isPaid)
    .reduce((sum, current) => sum + current.amount, 0);

  useEffect(() => {
    fetchPenalties();
  }, []);

  const fetchPenalties = async () => {
    try {
      setLoading(true);
      const response = await myProfileService.getMyPenalties();
      if (response.success) {
        setPenalties(response.data);
      }
    } catch (err) {
      setError("Cezalarınız yüklenirken bir hata oluştu.");
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="my-penalties-container">
        <div className="loading-text">Ceza ve işlem kayıtlarınız yükleniyor...</div>
      </div>
    );
  }

  return (
    <div className="my-penalties-container">
      <div className="page-header">
        <div>
          <h2>Cezalarım ve İşlemler</h2>
          <p>Kütüphane kuralları gereği tarafınıza yansıtılan gecikme, hasar veya kayıp kitap bedellerini buradan takip edebilirsiniz.</p>
        </div>
        {totalDebt > 0 && (
          <div className="debt-summary-card">
            <span>Toplam Ödenmemiş Borç</span>
            <strong>{formatMoney(totalDebt)}</strong>
          </div>
        )}
      </div>

      {error && <div className="alert-error">{error}</div>}

      {penalties.length > 0 ? (
        <div className="penalties-list">
          {penalties.map((penalty) => (
            <div key={penalty.id} className={`penalty-card ${penalty.isPaid ? 'is-paid' : 'is-unpaid'}`}>
              
              <div className="penalty-icon-wrapper">
                {getPenaltyIcon(penalty.penaltyType)}
              </div>
              
              <div className="penalty-details">
                <div className="penalty-header">
                  <h3>{penalty.penaltyType || 'Bilinmeyen Ceza'}</h3>
                  <span className={`status-badge ${penalty.isPaid ? 'badge-paid' : 'badge-unpaid'}`}>
                    {penalty.isPaid ? 'Ödendi' : 'Ödenmedi'}
                  </span>
                </div>
                
                <p className="penalty-book-info">
                  <strong>Kitap:</strong> {penalty.bookTitle || 'Belirtilmemiş'} 
                  {penalty.barcode && <span className="barcode-text"> (Barkod: {penalty.barcode})</span>}
                </p>
                
                <div className="penalty-meta">
                  <span>İşlem Tarihi: {formatDate(penalty.createdAt)}</span>
                  {penalty.isPaid && penalty.paidDate && (
                    <span className="paid-date">Tahsilat: {formatDate(penalty.paidDate)}</span>
                  )}
                </div>
              </div>

              <div className="penalty-amount-box">
                <span className="amount-label">Tutar</span>
                <span className={`amount-value ${penalty.isPaid ? 'text-success' : 'text-danger'}`}>
                  {formatMoney(penalty.amount)}
                </span>
                {/* Gelecekte Sanal POS eklenirse "Ödeme Yap" butonu buraya eklenebilir */}
              </div>

            </div>
          ))}
        </div>
      ) : (
        <div className="empty-state-card">
          <div className="empty-icon">✅</div>
          <h3>Harika! Hiç Cezanız Yok.</h3>
          <p>Kütüphane kurallarına uyduğunuz ve kitaplarınızı zamanında teslim ettiğiniz için teşekkür ederiz.</p>
        </div>
      )}
    </div>
  );
}