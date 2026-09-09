import React, { useState, useEffect } from 'react';
import { loanService, lostBookService } from '../../services/api';
import './LoanListAdminPage.css';

export default function LoanListAdminPage() {
  const [loans, setLoans] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

  // Kayıp Bildir Modal State'leri
  const [showLostModal, setShowLostModal] = useState(false);
  const [selectedLoanId, setSelectedLoanId] = useState(null);
  const [bookValue, setBookValue] = useState('');

  useEffect(() => {
    fetchLoans();
  }, []);

  const fetchLoans = async () => {
    try {
      setLoading(true);
      const response = await loanService.getActiveLoans(); 
      if (response.success) {
        setLoans(response.data);
      } else {
        setError(response.message);
      }
    } catch (err) {
      setError("Ödünç listesi yüklenirken bir hata oluştu.");
    } finally {
      setLoading(false);
    }
  };

  const handleReturn = async (id) => {
    if (window.confirm("Bu kitabın iadesini almak istediğinize emin misiniz?")) {
      try {
        const response = await loanService.returnLoan(id);
        if (response.success) {
          alert(response.message);
          fetchLoans();
        }
      } catch (err) {
        alert("İade işlemi sırasında bir hata oluştu.");
      }
    }
  };

  // Modal Açma
  const openLostModal = (id) => {
    setSelectedLoanId(id);
    setBookValue('');
    setShowLostModal(true);
  };

  // Modal Gönderme
  const handleLostSubmit = async () => {
    if (!bookValue || isNaN(bookValue) || Number(bookValue) <= 0) {
      alert("Lütfen geçerli bir kitap bedeli giriniz.");
      return;
    }

    try {
      const response = await lostBookService.reportLost({
        loanExternalId: selectedLoanId,
        bookValue: Number(bookValue)
      });
      
      if (response.success) {
        alert(response.message);
        setShowLostModal(false);
        fetchLoans(); // Listeyi yenile ki kitap "Aktif Ödünçler"den düşsün
      }
    } catch (err) {
      alert(err.response?.data?.message || "Kayıp bildirimi başarısız oldu.");
    }
  };

  const filteredLoans = loans.filter(loan => 
    loan.memberFullName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    loan.bookTitle?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    loan.barcode?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="loan-page-container">
      <div className="page-header">
        <div>
          <h2>Aktif Ödünç Listesi</h2>
          <p>Şu anda üyelerde bulunan ve iade bekleyen tüm kitapların listesi.</p>
        </div>
      </div>

      <div className="filter-bar">
        <input 
          type="text" 
          placeholder="Üye, kitap veya barkod ara..." 
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="search-input"
        />
      </div>

      {loading ? (
        <p className="loading-text">Yükleniyor...</p>
      ) : error ? (
        <p className="error-message">{error}</p>
      ) : (
        <div className="table-responsive">
          <table className="data-table">
            <thead>
              <tr>
                <th>Üye</th>
                <th>Kitap / Barkod</th>
                <th>Veriliş Tarihi</th>
                <th>Teslim Tarihi</th>
                <th>Durum / Gecikme</th>
                <th>İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {filteredLoans.length > 0 ? (
                filteredLoans.map((loan) => {
                  const badgeClass = loan.isOverdue ? 'badge-danger' : (loan.status === 'Kritik' ? 'badge-warning' : 'badge-normal');
                  const badgeText = loan.isOverdue ? `${loan.delayDays} Gün Gecikti` : loan.status;

                  return (
                    <tr key={loan.id}>
                      <td>
                        <div className="text-medium">{loan.memberFullName}</div>
                        <small className="text-muted">{loan.memberNumber}</small>
                      </td>
                      <td>
                        <div className="text-medium">{loan.bookTitle}</div>
                        <small className="text-muted">Barkod: {loan.barcode}</small>
                      </td>
                      <td>{new Date(loan.loanDate).toLocaleDateString('tr-TR')}</td>
                      <td>{new Date(loan.dueDate).toLocaleDateString('tr-TR')}</td>
                      <td>
                        <span className={`badge ${badgeClass}`}>
                          {badgeText}
                        </span>
                      </td>
                      <td>
                        <div className="loan-action-buttons">
                          <button className="btn-return" onClick={() => handleReturn(loan.id)}>
                            İade Al
                          </button>
                          <button className="btn-return btn-lost" onClick={() => openLostModal(loan.id)}>
                            Kayıp Bildir
                          </button>
                        </div>
                      </td>
                    </tr>
                  );
                })
              ) : (
                <tr>
                  <td colSpan="6" className="text-center padding-large">Kayıp veya kayıt bulunamadı.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      {/* KAYIP KİTAP MODALI */}
      {showLostModal && (
        <div className="lost-modal-overlay">
          <div className="lost-modal-content">
            <h3>Kayıp Kitap Bildirimi</h3>
            <p className="lost-modal-desc">
              Kitap kopyası sistemde 'Kayıp' olarak işaretlenecek ve üyenin hesabına aşağıdaki bedel ceza olarak yansıtılacaktır.
            </p>
            <div className="lost-modal-form-group">
              <label>Kitap Bedeli (₺)</label>
              <input 
                type="number" 
                value={bookValue} 
                onChange={(e) => setBookValue(e.target.value)}
                placeholder="Örn: 250.00"
              />
            </div>
            <div className="lost-modal-actions">
              <button className="btn-cancel" onClick={() => setShowLostModal(false)}>İptal</button>
              <button className="btn-confirm-lost" onClick={handleLostSubmit}>Onayla ve Ceza Kes</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}