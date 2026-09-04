import React, { useState, useEffect } from 'react';
import { bookService, bookCopyService } from '../../services/api';
import './AddBookCopyPage.css';

const AddBookCopyPage = () => {
    const [books, setBooks] = useState([]); 
    const [selectedBookDetails, setSelectedBookDetails] = useState(null); 

    const [formData, setFormData] = useState({
        bookId: '',
        barcode: '',
        shelfLocation: ''
    });

    useEffect(() => {
        const fetchBooks = async () => {
            try {
                const result = await bookService.getAll();
                if (result.success) {
                    setBooks(result.data);
                }
            } catch (error) {
                console.error("Kitaplar yüklenirken hata oluştu:", error);
            }
        };
        
        fetchBooks();
    }, []);

    const handleChange = async (e) => {
        const { name, value } = e.target;
        
        setFormData(prevState => ({
            ...prevState,
            [name]: value
        }));

        if (name === 'bookId') {
            try {
                const detailResult = await bookService.getById(value);
                if (detailResult.success) {
                    setSelectedBookDetails(detailResult.data);
                }
            } catch (error) {
                console.error("Kitap detayı çekilemedi:", error);
                setSelectedBookDetails(null);
            }
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        try {
            const dataToSend = {
                bookId: formData.bookId, 
                barcode: formData.barcode,
                shelfLocation: formData.shelfLocation
            };

            const result = await bookCopyService.addBookCopy(dataToSend);
            
            if(result.success) {
                alert("Kopya başarıyla eklendi!");
                setFormData({ bookId: '', barcode: '', shelfLocation: '' }); 
                setSelectedBookDetails(null); 
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
        setSelectedBookDetails(null); 
    };

    return (
        <div className="add-book-copy-container">
            <div className="add-book-copy-card">
                <div className="form-header">
                    <h2>Yeni Fiziksel Kopya Ekle</h2>
                    <p>Sisteme eklenecek yeni kitap kopyasının lokasyon ve barkod bilgilerini giriniz.</p>
                </div>

                {/* KİTAP ÖNİZLEME KARTI */}
                {selectedBookDetails && (
                    <div className="book-preview-card">
                        {/* Kapak Fotoğrafı */}
                        {selectedBookDetails.coverImageUrl ? (
                            <img src={selectedBookDetails.coverImageUrl} alt="Kapak" className="preview-cover-image" />
                        ) : (
                            <div className="preview-cover-fallback">Görsel Yok</div>
                        )}
                        
                        {/* Kitap Detayları */}
                        <div className="preview-details-container">
                            <h3 className="preview-title">{selectedBookDetails.title}</h3>
                            <p className="preview-text">
                                <strong>Yazar(lar):</strong> {selectedBookDetails.authors && selectedBookDetails.authors.length > 0 ? selectedBookDetails.authors.map(a => a.fullName).join(', ') : 'Belirtilmemiş'}
                            </p>
                            <p className="preview-text">
                                <strong>Yayınevi:</strong> {selectedBookDetails.publisher} ({selectedBookDetails.publicationYear})
                            </p>
                            <p className="preview-text">
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
                                        {book.title}
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