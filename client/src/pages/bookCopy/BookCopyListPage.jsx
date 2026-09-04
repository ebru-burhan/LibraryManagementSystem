import React, { useState, useEffect } from 'react';
import { bookCopyService } from '../services/api';

export default function BookCopyList() {
  const [copies, setCopies] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    bookCopyService.getAll()
      .then(res => {
        // API DataResult sarmalında döndüğü için veriyi res.data ile alıyoruz
        setCopies(res.data || res);
        setLoading(false);
      })
      .catch(err => {
        console.error("Kopyalar getirilemedi", err);
        setLoading(false);
      });
  }, []);

  if (loading) return <p>Yükleniyor...</p>;

  return (
    <div style={{ padding: '20px' }}>
      <h2>Kitap Kopyaları Listesi</h2>
      <table border="1" cellPadding="10" style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr style={{ background: '#f4f4f4' }}>
            <th>Barkod</th>
            <th>Kitap Adı</th>
            <th>Yazarlar</th>
            <th>Statü</th>
            <th>Eklenme Tarihi</th>
          </tr>
        </thead>
        <tbody>
          {copies.map(copy => (
            <tr key={copy.id}>
              <td>{copy.barcode}</td>
              <td>{copy.bookTitle}</td>
              <td>
                {/* DTO'da hazırladığımız string yazar listesini ekrana basıyoruz */}
                {copy.bookAuthorsNameList && copy.bookAuthorsNameList.join(', ')}
              </td>
              <td>
                {/* StatusName'e göre renklendirme yapılabilir (örn: AVAILABLE ise yeşil) */}
                <span style={{ fontWeight: 'bold', color: copy.statusName === 'AVAILABLE' ? 'green' : 'orange' }}>
                  {copy.statusName}
                </span>
              </td>
              <td>{new Date(copy.createdAt).toLocaleDateString()}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}