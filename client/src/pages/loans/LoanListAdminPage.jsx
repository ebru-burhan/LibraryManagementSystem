import React, { useState, useEffect } from 'react';
import { loanService } from '../../services/api';
import './LoanListAdminPage.css';

export default function LoanListAdminPage() {
  const [loans, setLoans] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

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
        fetchLoans(); // Listeyi yenile
      }
    } catch (err) {
      alert("İade işlemi sırasında bir hata oluştu.");
    }
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
        <p>Yükleniyor...</p>
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
                  // DTO'daki isOverdue ve delayDays ile rozet mantığını güvenli şekilde türetiyoruz
                  const badgeClass = loan.isOverdue ? 'badge-danger' : (loan.status === 'Kritik' ? 'badge-warning' : 'badge-normal');
                  const badgeText = loan.isOverdue ? `${loan.delayDays} Gün Gecikti` : loan.status;

                  return (
                    <tr key={loan.id}>
                      <td>
                        <div style={{ fontWeight: '500' }}>{loan.memberFullName}</div>
                        <small style={{ color: '#666' }}>{loan.memberNumber}</small>
                      </td>
                      <td>
                        <div style={{ fontWeight: '500' }}>{loan.bookTitle}</div>
                        <small style={{ color: '#666' }}>Barkod: {loan.barcode}</small>
                      </td>
                      <td>{new Date(loan.loanDate).toLocaleDateString('tr-TR')}</td>
                      <td>{new Date(loan.dueDate).toLocaleDateString('tr-TR')}</td>
                      <td>
                        <span className={`badge ${badgeClass}`}>
                          {badgeText}
                        </span>
                      </td>
                      <td>
                        <button 
                            className="btn-return" 
                            onClick={() => handleReturn(loan.id)}
                            >
                            İade Al
                        </button>
                      </td>
                    </tr>
                  );
                })
              ) : (
                <tr>
                  <td colSpan="6" style={{ textAlign: 'center' }}>Kayıt bulunamadı.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}