import React, { useState, useEffect } from 'react';
import { bookService, bookCopyService } from '../../services/api';
import './AddBookCopyPage.css';

const AddBookCopyPage = () => {
    const [books, setBooks] = useState([]); 
    
    // YENİ: Seçilen kitabın tüm bilgilerini (resim, yazar vb.) tutacak state
    const [selectedBookDetails, setSelectedBookDetails] = useState(null); 

    const [formData, setFormData] = useState({
        bookId: '',
        barcode: '',
        shelfLocation: ''
    });

    useEffect(() => {
        const fetchBooks = async () => {
            try {
                const result = await bookService.getAllBooks();
                if (result.success) {
                    setBooks(result.data);
                }
            } catch (error) {
                console.error("Kitaplar yüklenirken hata oluştu:", error);
            }
        };
        
        fetchBooks();
    }, []);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prevState => ({
            ...prevState,
            [name]: value
        }));

        // YENİ: Eğer değişen alan Dropdown (bookId) ise, kitabın detaylarını bul ve kartı çiz
        if (name === 'bookId') {
            const selectedBook = books.find(b => b.id === parseInt(value));
            setSelectedBookDetails(selectedBook);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        try {
            const dataToSend = {
                bookId: parseInt(formData.bookId), 
                barcode: formData.barcode,
                shelfLocation: formData.shelfLocation
            };

            const result = await bookCopyService.add(dataToSend);
            
            if(result.success) {
                alert("Kopya başarıyla eklendi!");
                setFormData({ bookId: '', barcode: '', shelfLocation: '' }); 
                setSelectedBookDetails(null); // YENİ: Başarılı kayıttan sonra önizleme kartını temizle
            } else {
                alert("Hata: " + result.message);
            }
        } catch (error) {
            console.error("API Hatası:", error);
            alert("Bağlantı sırasında bir hata oluştu.");
        }
    };

    const handleClear = () => {
        setFormData({ bookId: '', barcode: '', shelfLocation: '' });
        setSelectedBookDetails(null); // Temizle butonuna basınca kartı da uçur
    };

    return (
        <div className="add-book-copy-container">
            <div className="add-book-copy-card">
                <div className="form-header">
                    <h2>Yeni Fiziksel Kopya Ekle</h2>
                    <p>Sisteme eklenecek yeni kitap kopyasının lokasyon ve barkod bilgilerini giriniz.</p>
                </div>

                {/* YENİ: KİTAP ÖNİZLEME KARTI (Sadece kitap seçilince görünür) */}
                {selectedBookDetails && (
                    <div className="book-preview-card" style={{ display: 'flex', gap: '20px', marginBottom: '30px', padding: '15px', backgroundColor: '#fff4ed', borderRadius: '8px', border: '1px solid #ffe3d1' }}>
                        {/* Kapak Fotoğrafı */}
                        {selectedBookDetails.coverImageUrl ? (
                            <img src={selectedBookDetails.coverImageUrl} alt="Kapak" style={{ width: '80px', height: '120px', objectFit: 'cover', borderRadius: '4px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' }} />
                        ) : (
                            <div style={{ width: '80px', height: '120px', backgroundColor: '#e0e0e0', borderRadius: '4px', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '12px', color: '#666' }}>Görsel Yok</div>
                        )}
                        
                        {/* Kitap Detayları */}
                        <div style={{ display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
                            <h3 style={{ margin: '0 0 8px 0', color: '#333', fontSize: '18px' }}>{selectedBookDetails.title}</h3>
                            <p style={{ margin: '0 0 4px 0', fontSize: '14px', color: '#666' }}>
                                <strong>Yazar(lar):</strong> {selectedBookDetails.authors && selectedBookDetails.authors.length > 0 ? selectedBookDetails.authors.join(', ') : 'Belirtilmemiş'}
                            </p>
                            <p style={{ margin: '0 0 4px 0', fontSize: '14px', color: '#666' }}>
                                <strong>Yayınevi:</strong> {selectedBookDetails.publisher} ({selectedBookDetails.publicationYear})
                            </p>
                            <p style={{ margin: '0', fontSize: '14px', color: '#666' }}>
                                <strong>ISBN:</strong> {selectedBookDetails.isbn}
                            </p>
                        </div>
                    </div>
                )}

                <form onSubmit={handleSubmit} className="add-book-copy-form">
                    <div className="form-section">
                        
                        <div className="form-group-full">
                            <label>Ait Olduğu Kitap</label>
                            <select 
                                name="bookId" 
                                value={formData.bookId} 
                                onChange={handleChange} 
                                required
                            >
                                <option value="" disabled>Lütfen bir kitap seçiniz...</option>
                                {books.map(book => (
                                    <option key={book.id} value={book.id}>
                                        {book.title} (ISBN: {book.isbn})
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className="form-group-row">
                            <div className="form-group">
                                <label>Barkod</label>
                                <input 
                                    type="text" 
                                    name="barcode" 
                                    value={formData.barcode} 
                                    onChange={handleChange} 
                                    placeholder="Örn: BRC-123456" 
                                    maxLength="50"
                                    required 
                                />
                            </div>
                            <div className="form-group">
                                <label>Raf Konumu</label>
                                <input 
                                    type="text" 
                                    name="shelfLocation" 
                                    value={formData.shelfLocation} 
                                    onChange={handleChange} 
                                    placeholder="Örn: A-Blok, 3. Raf" 
                                    maxLength="50"
                                    required 
                                />
                            </div>
                        </div>

                    </div>

                    <div className="form-actions">
                        <button type="button" className="btn-cancel" onClick={handleClear}>Temizle</button>
                        <button type="submit" className="btn-submit">Kaydet</button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default AddBookCopyPage;