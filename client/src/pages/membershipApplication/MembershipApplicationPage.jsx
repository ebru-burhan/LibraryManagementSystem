import React, { useState, useEffect } from 'react';

import { membershipService, myProfileService } from '../../services/api';
import { useAuth } from '../../hooks/useAuth';
import './MembershipApplicationPage.css';

const FILE_BASE = 'https://localhost:7213';

export default function MembershipApplicationPage() {
const { firstName, lastName, email, isMember } = useAuth();
  
  // Sayfa Durum Yönetimi: LOADING, FORM, PENDING, REJECTED, APPROVED
  const [viewState, setViewState] = useState('LOADING');
  const [appData, setAppData] = useState(null);

  // Form State
  const [formData, setFormData] = useState({
    identityNumber: '',
    dateOfBirth: '',
    phoneNumber: '',
    address: '',
    membershipTypeCode: ''
  });
  const [pictureFile, setPictureFile] = useState(null);
  const [previewUrl, setPreviewUrl] = useState(null);
  const [documentFile, setDocumentFile] = useState(null);

  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchMyStatus();
  }, []);

const fetchMyStatus = async () => {
    try {
      setViewState('LOADING');

      // 1. EĞER KULLANICI ONAYLI ÜYEYSE DOĞRUDAN GÜNCEL MEMBER PROFİLİNİ ÇEK
      if (isMember) {
        const response = await myProfileService.getMyDetails();
        if (response.success && response.data) {
          setAppData(response.data);
          setViewState('APPROVED');
          return; // İşlemi burada kes
        }
      }

      // 2. EĞER ÜYE DEĞİLSE BAŞVURU DURUMUNA BAK (Eski mantık)
      const response = await membershipService.getMyStatus();
      if (response.success && response.data) {
        setAppData(response.data);
        const status = response.data.applicationStatus?.toUpperCase();
        
        if (status === 'PENDING') setViewState('PENDING');
        else if (status === 'REJECTED') setViewState('REJECTED');
        else setViewState('FORM'); 

      } else {
        setViewState('FORM');
      }
    } catch (err) {
      console.error(err);
      // 2. DÜZELTME: Eğer kişiyse form yerine hata mesajı/boş ekran göster
      if (isMember) {
        setError("Kütüphane profilinize şu an ulaşılamıyor.");
        setViewState('ERROR'); // Yeni Error state'ine at
      } else {
        setViewState('FORM');
      }
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handlePictureChange = (e) => {
    if (e.target.files && e.target.files[0]) {
      const file = e.target.files[0];
      setPictureFile(file);
      setPreviewUrl(URL.createObjectURL(file));
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
    data.append('MembershipTypeCode', formData.membershipTypeCode);
    
    if (pictureFile) data.append('pictureFile', pictureFile);
    if (documentFile) data.append('documentFile', documentFile);

    try {
      const response = await membershipService.apply(data);
      setMessage(response.message || "Başvurunuz başarıyla alındı!");
      
      // Başarılı olunca 1.5 saniye sonra durumu yeniden çek (Beklemede ekranına geçsin)
      setTimeout(() => {
        fetchMyStatus();
      }, 1500);
    } catch (err) {
      setError(err.response?.data?.message || "Başvuru sırasında bir hata oluştu.");
      setLoading(false);
    }
  };

  // ================= EKRAN RENDER METOTLARI =================

  const renderForm = () => (
    <div className="application-form-card">
      <div className="form-header">
        <h2>Üyelik Başvuru Formu</h2>
        <p>Kütüphane hizmetlerinden yararlanmak için profilinizi tamamlayın.</p>
      </div>

      {message && <div className="alert alert-success">{message}</div>}
      {error && <div className="alert alert-error">{error}</div>}

      <form onSubmit={handleSubmit} className="app-form">
        
        <div className="form-row upload-row">
          <div className="avatar-upload">
            <img 
              src={previewUrl || "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_960_720.png"} 
              alt="Profil Önizleme" 
            />
            <div className="upload-controls">
              <label>Profil Fotoğrafı</label>
              <input type="file" accept="image/*" onChange={handlePictureChange} />
            </div>
          </div>
        </div>

        <div className="form-row split-2">
          <div className="input-group">
            <label>Ad (Hesap Bilgisi)</label>
            <input type="text" value={firstName || ''} disabled className="disabled-input" />
          </div>
          <div className="input-group">
            <label>Soyad (Hesap Bilgisi)</label>
            <input type="text" value={lastName || ''} disabled className="disabled-input" />
          </div>
        </div>

        <div className="form-row split-2">
          <div className="input-group">
            <label>E-posta (Hesap Bilgisi)</label>
            <input type="email" value={email || ''} disabled className="disabled-input" />
          </div>
          <div className="input-group">
            <label>Üyelik Tipi *</label>
            <select name="membershipTypeCode" value={formData.membershipTypeCode} onChange={handleChange} required>
              <option value="" disabled>Seçiniz...</option>
              <option value="STUDENT">Öğrenci</option>
              <option value="ACADEMIC">Akademik Personel</option>
              <option value="PUBLIC">Sivil / Halk</option>
            </select>
          </div>
        </div>

        <div className="form-row split-2">
          <div className="input-group">
            <label>T.C. Kimlik Numarası *</label>
            <input type="text" name="identityNumber" maxLength={11} value={formData.identityNumber} onChange={handleChange} required />
          </div>
          <div className="input-group">
            <label>Doğum Tarihi *</label>
            <input type="date" name="dateOfBirth" value={formData.dateOfBirth} onChange={handleChange} required />
          </div>
        </div>

        <div className="form-row split-2">
          <div className="input-group">
            <label>Telefon Numarası *</label>
            <input type="text" name="phoneNumber" placeholder="05XXXXXXXXX" value={formData.phoneNumber} onChange={handleChange} required />
          </div>
          <div className="input-group">
            <label>Ek Belge (Öğrenci/Personel Kartı)</label>
            <input type="file" accept=".pdf,image/*" onChange={handleDocumentChange} />
          </div>
        </div>

        <div className="form-row">
          <div className="input-group">
            <label>Adres</label>
            <textarea name="address" placeholder="Açık adresiniz..." value={formData.address} onChange={handleChange} rows="3" />
          </div>
        </div>
        
        <div className="form-actions">
          <button type="submit" disabled={loading} className="btn-submit">
            {loading ? 'İşleniyor...' : 'Başvuruyu Tamamla'}
          </button>
        </div>
      </form>
    </div>
  );

  const renderPending = () => (
    <div className="status-card pending">
      <div className="status-icon">⏳</div>
      <h2>Başvurunuz Değerlendiriliyor</h2>
      <p>Kütüphane yöneticileri başvurunuzu inceliyor. İşlem tamamlandığında kütüphane hizmetlerinden yararlanmaya başlayabilirsiniz.</p>
      <div className="status-meta">
        <span>Başvuru Tarihi: {new Date(appData?.createdAt).toLocaleDateString('tr-TR')}</span>
        <span>Üyelik Tipi: {appData?.membershipType?.name}</span>
      </div>
    </div>
  );

  const renderRejected = () => (
    <div className="status-card rejected">
      <div className="status-icon">❌</div>
      <h2>Başvurunuz Reddedildi</h2>
      <p>Başvurunuz kütüphane politikalarına uygun bulunmadığı veya eksik belge nedeniyle reddedilmiştir.</p>
      <button 
        className="btn-retry" 
        onClick={() => {
          // Yeniden başvurmak için formu pre-fill yap
          setFormData({
            identityNumber: appData?.identityNumber || '',
            dateOfBirth: appData?.dateOfBirth ? appData.dateOfBirth.split('T')[0] : '',
            phoneNumber: appData?.phoneNumber || '',
            address: appData?.address || '',
            membershipTypeCode: appData?.membershipType?.code || ''
          });
          setViewState('FORM');
        }}
      >
        Bilgileri Güncelle ve Tekrar Başvur
      </button>
    </div>
  );

 const renderApproved = () => (
    <div className="my-profile-container">
      <div className="member-profile-card">
        {/* Sol Taraf: Profil Fotoğrafı */}
        {appData?.pictureUrl ? (
          <img src={`${FILE_BASE}${appData.pictureUrl}`} alt="Üye" />
        ) : (
          <div className="member-profile-fallback">{appData?.firstName?.charAt(0)}</div>
        )}
        
        {/* Sağ Taraf: Bilgiler */}
        <div className="member-profile-info">
          <div className="profile-top-row">
            <div>

             <div className="member-profile-title">
                <h2>{appData?.fullName || `${appData?.firstName} ${appData?.lastName}`}</h2>
                <span className={`member-detail-badge ${appData?.status === 'ACTIVE' ? 'badge-active' : 'badge-suspended'}`}>
                  {appData?.statusName || 'Aktif Değil'}
                </span>
              </div>

              <p className="profile-id-text">
                  T.C: {appData?.identityNumber} • {appData?.membershipType?.name}
                </p>

            </div>
          </div>

          <hr className="profile-divider" />

          <div className="profile-meta-row">

            <div className="meta-item">
              <span>E-posta</span>
              <strong>{appData?.email}</strong>
            </div>

           <div className="meta-item">
            <span>Telefon</span>
            <strong>{appData?.phoneNumber || '—'}</strong>
          </div>

            <div className="meta-item">
              <span>Kayıt Tarihi</span>
              <strong>{new Date(appData?.createdAt).toLocaleDateString('tr-TR')}</strong>
            </div>

            <div className="meta-item">
              <span>Adres</span>
              <strong>{appData?.address || '—'}</strong>
            </div>

          </div>
        </div>
      </div>
      
      {/* Güncelleme: Pasif üyelere uyarı mesajı */}
      <p className="profile-footer-note">
        {appData?.status === 'ACTIVE' 
          ? "Kütüphane hizmetlerinden (ödünç alma, rezervasyon) aktif olarak faydalanabilirsiniz."
          : "Üyeliğiniz şu anda pasif veya askıya alınmış durumdadır. İşlem yapabilmek için lütfen kütüphane yönetimiyle iletişime geçiniz."}
      </p>
      
    </div>
  );

const renderError = () => (
    <div className="status-card error">
      <div className="status-icon">⚠️</div>
      <h2>Sistem Hatası</h2>
      <p>{error}</p>
    </div>
  );

  return (
    <div className="membership-page">
      {viewState === 'LOADING' && <div className="loading-state">Durumunuz kontrol ediliyor...</div>}
      {viewState === 'FORM' && renderForm()}
      {viewState === 'PENDING' && renderPending()}
      {viewState === 'REJECTED' && renderRejected()}
      {viewState === 'APPROVED' && renderApproved()}
      {viewState === 'ERROR' && renderError()}
    </div>
  );
}