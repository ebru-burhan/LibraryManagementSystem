import React, { useState, useEffect } from 'react';
import { lostBookService } from '../../services/api';
import './LostBookListAdminPage.css'; // Kendi CSS dosyasını bağlıyoruz

export default function LostBookListAdminPage() {
  const [lostBooks, setLostBooks] = useState([]);
  const [kpis, setKpis] = useState({ totalLostBooks: 0, totalUnpaidAmount: 0, monthlyReports: 0 });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const [kpiRes, listRes] = await Promise.all([
          lostBookService.getKpis(),
          lostBookService.getAll()
        ]);
        
        if (kpiRes.success) setKpis(kpiRes.data);
        if (listRes.success) setLostBooks(listRes.data);
      } catch (error) {
        console.error("Veriler çekilirken hata:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) return <div className="page-loading">Yükleniyor...</div>;

  return (
    <div className="member-directory-page">
      <div className="member-directory-header">
        <div>
          <h2>Kayıp Kitaplar</h2>
          <p>Kayıp olarak işaretlenen kitapların mali takibi ve üye bildirimleri.</p>
        </div>
      </div>

      <div className="member-stat-grid kpi-margin">
        <article className="member-stat-card">
          <span>Toplam Kayıp Kitap</span>
          <strong className="text-danger">{kpis.totalLostBooks}</strong>
        </article>
        <article className="member-stat-card attention">
          <span>Tahsil Edilmeyen Tutar</span>
          <strong>{Number(kpis.totalUnpaidAmount).toFixed(2)} ₺</strong>
        </article>
        <article className="member-stat-card">
          <span>Bu Ayki Bildirimler</span>
          <strong className="text-primary">{kpis.monthlyReports}</strong>
        </article>
      </div>

      <div className="table-responsive">
        <table className="admin-table">
          <thead>
            <tr>
              <th>Üye</th>
              <th>Kitap</th>
              <th>Bildirim Tarihi</th>
              <th>Kitap Bedeli</th>
              <th>Tahsilat Durumu</th>
            </tr>
          </thead>
          <tbody>
            {lostBooks.length > 0 ? (
              lostBooks.map((lb) => (
                <tr key={lb.id}>
                  <td>
                    <div className="text-bold">{lb.memberFullName}</div>
                    <small className="text-muted">ID: {lb.memberNumber}</small>
                  </td>
                  <td>
                    <div className="text-bold">{lb.bookTitle}</div>
                    <small className="text-muted">ISBN: {lb.isbn}</small>
                  </td>
                  <td>{new Date(lb.declaredDate).toLocaleDateString('tr-TR')}</td>
                  <td className="text-bold">{Number(lb.bookValue).toFixed(2)} ₺</td>
                  <td>
                    <span className={`badge ${lb.isResolved ? 'badge-resolved' : 'badge-pending'}`}>
                      {lb.isResolved ? 'Ödendi' : 'Bekliyor'}
                    </span>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="5" className="td-empty padding-large">
                  Kayıp kitap kaydı bulunamadı.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}