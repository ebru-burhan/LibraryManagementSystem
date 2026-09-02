import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { membershipService } from '../../services/api';
import { useAuth } from '../../hooks/useAuth';
import './MembershipApplicationPage.css';

export default function MembershipApplicationPage() {
  const navigate = useNavigate();
  const { firstName, lastName, email } = useAuth(); 

  const [formData, setFormData] = useState({
    identityNumber: '',
    dateOfBirth: '',
    phoneNumber: '',
    address: '',
  });

  const [pictureFile, setPictureFile] = useState(null);
  const [previewUrl, setPreviewUrl] = useState(null);

// Ekstra Belge (Kimlik/PDF vb.) İçin YENİ STATE
  const [documentFile, setDocumentFile] = useState(null);

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

const handlePictureChange = (e) => {
    console.log("Seçilen dosyalar:", e.target.files); // Dosya geliyor mu kontrol edelim
    if (e.target.files && e.target.files[0]) {
      const file = e.target.files[0];
      setPictureFile(file);
      const url = URL.createObjectURL(file);
      console.log("Oluşturulan Önizleme URL:", url); // URL üretiliyor mu bakalım
      setPreviewUrl(url);
    }
  };


  const handleDocumentChange = (e) => {
    if (e.target.files && e.target.files[0]) {
      setDocumentFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setMessage(null);

    const data = new FormData();
    data.append('identityNumber', formData.identityNumber);
    data.append('dateOfBirth', formData.dateOfBirth);
    data.append('phoneNumber', formData.phoneNumber);
    data.append('address', formData.address);
    
    if (pictureFile) {
      data.append('pictureFile', pictureFile);
    }

    if (documentFile) {
      data.append('documentFile', documentFile);
    }

    try {
      const response = await membershipService.apply(data);
      
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
       {/* PROFİL RESMİ VE ÖNİZLEME ALANI */}
        <div className="input-group">
          <label>Profil Resmi</label>
          <div style={{ display: 'flex', alignItems: 'center', gap: '15px', marginTop: '0.5rem' }}>
            {/* Önizleme veya Varsayılan Görsel (Her zaman görünür) */}
            <div className="preview-container" style={{ marginTop: 0 }}>
              <img 
                src={previewUrl || "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_960_720.png"} 
                alt="Profil Önizleme" 
                className="preview-image" 
              />
            </div>
            
            {/* Dosya Seçme Butonu */}
            <input 
              type="file" 
              accept="image/*" 
              onChange={handlePictureChange} 
              className="file-input-field"
            />
          </div>
        </div>

        {/* SADECE OKUNABİLİR HESAP BİLGİLERİ */}
        <div className="input-group">
          <label>Ad (Hesap Bilgisi)</label>
          <input 
            type="text" 
            value={firstName || ''} 
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

        <div className="input-group">
          <label>Ek Dosya</label>
          <div style={{ display: 'flex', alignItems: 'center', gap: '15px', marginTop: '0.5rem' }}>
       
            {/* Dosya Seçme Butonu */}
            <input 
              type="file" 
              accept=".pdf,image/*" 
              onChange={handleDocumentChange}
              className="file-input-field"
            />
          </div>
        </div>
        
        <button type="submit" disabled={loading} className="submit-btn">
          {loading ? 'Gönderiliyor...' : 'Başvuruyu Gönder'}
        </button>
      </form>
    </div>
  );
}