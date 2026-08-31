import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { membershipService } from '../../services/api';
import { useAuth } from '../../hooks/useAuth'; // Token'dan bilgileri çeken hook
import './MembershipApplicationPage.css';

export default function MembershipApplicationPage() {
  const navigate = useNavigate();
  
  // useAuth içinden token'da bulunan mevcut kullanıcı bilgilerini çekiyoruz
  const { firstName, lastName, email } = useAuth(); 

  const [formData, setFormData] = useState({
    identityNumber: '',
    dateOfBirth: '',
    phoneNumber: '',
    address: '',
  });

  const [message, setMessage] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setMessage(null);

    // DTO sadece kullanıcının girdiği eksik bilgileri bekliyor
    const payload = {
      identityNumber: formData.identityNumber,
      dateOfBirth: formData.dateOfBirth,
      phoneNumber: formData.phoneNumber,
      address: formData.address,
    };

    try {
      const response = await membershipService.apply(payload);
      
      // Başarılı olduğunda onay mesajı gösterip Dashboard'a yönlendiriyoruz
      setMessage(response.message || "Başvurunuz başarıyla alındı! Durum paneline yönlendiriliyorsunuz...");
      
      setTimeout(() => {
        navigate('/dashboard'); 
      }, 1500);

    } catch (err) {
      setError(err.response?.data?.message || "Başvuru sırasında bir hata oluştu.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="application-container">
      <h2>Üyelik Başvuru Formu</h2>
      {message && <div className="alert-success">{message}</div>}
      {error && <div className="alert-error">{error}</div>}

      <form onSubmit={handleSubmit}>
        
        {/* SADECE OKUNABİLİR HESAP BİLGİLERİ */}
        <div className="input-group">
          <label>Ad (Hesap Bilgisi)</label>
          <input 
            type="text" 
            value={firstName || ''} // useAuth'tan gelen değeri basıyoruz
            disabled 
            className="input-field disabled-input" 
          />
        </div>

        <div className="input-group">
          <label>Soyad (Hesap Bilgisi)</label>
          <input 
            type="text" 
            value={lastName || ''} 
            disabled 
            className="input-field disabled-input" 
          />
        </div>

        <div className="input-group">
          <label>E-posta (Hesap Bilgisi)</label>
          <input 
            type="email" 
            value={email || ''} 
            disabled 
            className="input-field disabled-input" 
          />
        </div>

        {/* KULLANICININ DOLDURACAĞI ALANLAR */}
        <div className="input-group">
          <label>T.C. Kimlik Numarası</label>
          <input 
            type="text" 
            name="identityNumber" 
            maxLength={11} 
            value={formData.identityNumber} 
            onChange={handleChange} 
            required 
            className="input-field"
          />
        </div>

        <div className="input-group">
          <label>Doğum Tarihi</label>
          <input 
            type="date" 
            name="dateOfBirth" 
            value={formData.dateOfBirth} 
            onChange={handleChange} 
            required 
            className="input-field"
          />
        </div>

        <div className="input-group">
          <label>Telefon Numarası</label>
          <input 
            type="text" 
            name="phoneNumber" 
            placeholder="05XXXXXXXXX" 
            value={formData.phoneNumber} 
            onChange={handleChange} 
            required 
            className="input-field"
          />
        </div>

        <div className="input-group">
          <label>Adres</label>
          <textarea 
            name="address" 
            placeholder="Açık adresiniz..." 
            value={formData.address} 
            onChange={handleChange} 
            className="input-field"
          />
        </div>
        
        <button type="submit" disabled={loading} className="submit-btn">
          {loading ? 'Gönderiliyor...' : 'Başvuruyu Gönder'}
        </button>
      </form>
    </div>
  );
}