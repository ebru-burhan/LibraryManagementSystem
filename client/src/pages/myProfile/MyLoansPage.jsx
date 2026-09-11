import React, { useState, useEffect } from 'react';
import { myProfileService } from '../../services/api';
import './MyLoansPage.css';

// Tarih formatlayıcı
function formatDate(dateString) {
  if (!dateString) return '—';
  return new Date(dateString).toLocaleDateString('tr-TR');
}

// Backend'den gelen statüye göre CSS sınıfı seçimi
function getBadgeClass(status) {
  if (status === 'İade Edildi') return 'badge-returned';
  if (status === 'Gecikti') return 'badge-overdue';
  if (status === 'Kritik') return 'badge-warning';
  return 'badge-active'; // Normal
}

// Kalan gün metnini oluşturma
function getDaysText(dueDate, returnDate, isOverdue, delayDays) {
  if (returnDate) return '';
  if (isOverdue) return delayDays > 0 ? `${delayDays} Gün Gecikti!` : 'İade tarihi geçti!';
  
  const today = new Date();
  const due = new Date(dueDate);
  today.setHours(0, 0, 0, 0);
  due.setHours(0, 0, 0, 0);
  const diffDays = Math.ceil((due - today) / (1000 * 60 * 60 * 24));
  
  if (diffDays === 0) return 'Bugün Son';
  return `${diffDays} Gün Kaldı`;
}

export default function MyLoansPage() {
  const [loans, setLoans] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchLoans();
  }, []);

  const fetchLoans = async () => {
    try {
      setLoading(true);
      const response = await myProfileService.getMyLoans();
      if (response.success) {
        setLoans(response.data);
      }
    } catch (err) {
      setError("Ödünç bilgileri yüklenirken bir hata oluştu.");
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="my-loans-container"><div className="loading-text">Ödünç aldığınız kitaplar yükleniyor...</div></div>;
  }

  return (
    <div className="my-loans-container">
      <div className="page-header">
        <div>
          <h2>Ödünç Aldıklarım</h2>
          <p>Kütüphaneden aldığınız kitapların güncel durumunu ve iade tarihlerini buradan takip edebilirsiniz.</p>
        </div>
      </div>

      {error && <div className="alert-error">{error}</div>}

      {loans.length > 0 ? (
        <div className="loans-grid">
          {loans.map((loan) => {
            const badgeClass = getBadgeClass(loan.status);
            const daysText = getDaysText(loan.dueDate, loan.returnDate, loan.isOverdue, loan.delayDays);
            
            return (
              <div key={loan.id} className={`loan-card ${loan.returnDate ? 'is-returned' : ''} ${loan.isOverdue && !loan.returnDate ? 'is-danger' : ''}`}>
                <div className="loan-card-header">
                  <div className="book-title-area">
                    <span className="book-icon">📖</span>
                    <div>
                      <h3>{loan.bookTitle}</h3>
                      {/* Backend'den gelen DTO'da Author ekli değilse hata vermemesi için */}
                      <p className="book-author">{loan.authors || 'Genel Koleksiyon'}</p> 
                    </div>
                  </div>
                  {/* Backend'in döndüğü birebir statü yazdırılıyor (Normal, Kritik, Gecikti) */}
                  <span className={`status-badge ${badgeClass}`}>
                    {loan.status}
                  </span>
                </div>

                <div className="loan-dates">
                  <div className="date-box">
                    <span className="date-label">Ödünç Tarihi</span>
                    <span className="date-value">{formatDate(loan.loanDate)}</span>
                  </div>
                  <div className="date-box">
                    <span className="date-label">Son İade Tarihi</span>
                    <span className={`date-value ${loan.isOverdue && !loan.returnDate ? 'text-danger' : ''}`}>
                      {formatDate(loan.dueDate)}
                    </span>
                  </div>
                </div>

                <div className="loan-card-footer">
                  {loan.returnDate ? (
                    <div className="return-info">
                      İade İşlemi Tamamlandı: <strong>{formatDate(loan.returnDate)}</strong>
                    </div>
                  ) : (
                    <div className="action-area">
                      <span className={`days-left ${loan.isOverdue ? 'text-danger' : ''}`}>
                        ⏳ {daysText}
                      </span>
                    </div>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      ) : (
        <div className="empty-state-card">
          <div className="empty-icon">📚</div>
          <h3>Henüz Hiç Kitap Almadınız</h3>
          <p>Kütüphane kataloğunu inceleyerek ilginizi çeken kitapları keşfedebilir ve hemen ödünç alabilirsiniz.</p>
        </div>
      )}
    </div>
  );
}