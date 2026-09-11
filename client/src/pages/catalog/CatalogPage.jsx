import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { bookService, reservationService } from '../../services/api';
import { useAuth } from '../../hooks/useAuth';
import { PERMISSIONS } from '../../auth/permissionKeys';
import './CatalogPage.css'; // Sayfaya özel grid css'leri

const FILE_BASE = 'https://localhost:7213';

export default function CatalogPage() {
  const navigate = useNavigate();
  const { permissions, isMember } = useAuth();
  const canManageCatalog = permissions.includes(PERMISSIONS.MANAGE_CATALOG);

  const [books, setBooks] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchBooks();
  }, []);

  const fetchBooks = async (search = '') => {
    try {
      setLoading(true);
      const response = await bookService.getAll(search);
      if (response.success) {
        setBooks(response.data);
      }
    } catch (error) {
      console.error("Katalog yüklenirken hata:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = (e) => {
    e.preventDefault();
    fetchBooks(searchTerm);
  };

  const handleReserve = async (bookId) => { // Buradaki parametre adının önemi yok, nesne anahtarı önemli
    const confirmed = window.confirm("Bu kitap için rezervasyon sırasına girmek istiyor musunuz?");
    if (!confirmed) return;

    try {
      // DİKKAT: 'bookId' yerine backend'in beklediği 'bookExternalId' gönderiliyor!
      const response = await reservationService.create({ bookExternalId: bookId }); 
      
      if (response.success) {
        alert("Rezervasyonunuz başarıyla oluşturuldu! Sıranızı 'Rezervasyonlarım' sekmesinden takip edebilirsiniz.");
        fetchBooks(searchTerm); 
      }
    } catch (error) {
      alert(error.response?.data?.message || "Rezervasyon işlemi başarısız.");
    }
  };
  return (
    <div className="catalog-page">
      <div className="catalog-header">
        <div>
          <h2>Kütüphane Kataloğu</h2>
          <p>Aradığınız kitapları keşfedin ve rezervasyon yapın.</p>
        </div>
        
        {/* Sadece Kütüphaneciler (Admin/Librarian) görebilir */}
        {canManageCatalog && (
          <button 
            onClick={() => navigate('/book-copies/add')}
            className="btn-add-copy"
          >
            + Yeni Fiziksel Kopya Ekle
          </button>
        )}
      </div>

      {/* Arama Çubuğu */}
      <form onSubmit={handleSearch} className="catalog-search-bar">
        <input 
          type="text" 
          placeholder="Kitap Adı, Yazar veya ISBN ile ara..." 
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
        <button type="submit">Ara</button>
      </form>

      {/* Kitap Grid Listesi */}
      {loading ? (
        <div className="loading-text">Katalog yükleniyor...</div>
      ) : (
        <div className="book-grid">
          {books.length > 0 ? (
            books.map((book) => (
              <div key={book.id} className="book-card">
                
                {/* Kitap Kapağı */}
                <div className="book-cover">
                  {book.coverImageUrl ? (
                    <img src={`${FILE_BASE}${book.coverImageUrl}`} alt={book.title} />
                  ) : (
                    <div className="cover-fallback">Görsel Yok</div>
                  )}
                </div>

                {/* Kitap Bilgileri */}
                <div className="book-info">
                  <span className="book-category">{book.categoryName || 'Genel'}</span>
                  <h3 className="book-title">{book.title}</h3>
                  <p className="book-author">
                    {book.authors && book.authors.length > 0 
                      ? book.authors.map(a => a.fullName).join(', ') 
                      : 'Bilinmeyen Yazar'}
                  </p>

              {/* Butonlar ve Durum Kontrolü */}
                  <div className="book-actions">
                    {/* İncele Butonu - Herkese Açık */}
                    <button 
                      className="btn-inspect"
                      onClick={() => navigate(`/catalog/${book.id}`)}
                    >
                      İncele
                    </button>

                    {/* Stok Durumu ve Rezervasyon (Sadece Üyeler İçin) */}
                    {book.availableCopiesCount > 0 ? (
                      <span className="status-badge available">Rafta ({book.availableCopiesCount} Adet)</span>
                    ) : (
                      /* Stok yoksa ve SADECE isMember ise Rezervasyon Butonunu göster. Değilse hiçbir şey gösterme. */
                      isMember && (
                        <button 
                          className="btn-reserve"
                          onClick={() => handleReserve(book.id)}
                        >
                          Rezervasyon Yap
                        </button>
                      )
                    )}
                  </div>
                </div>
              </div>
            ))
          ) : (
            <p className="empty-catalog">Aradığınız kriterlere uygun kitap bulunamadı.</p>
          )}
        </div>
      )}
    </div>
  );
}