import React, { useState } from 'react';
import { authService } from '../../services/api';
import { useNavigate, Link } from 'react-router-dom';
import './RegisterPage.css'; 

export default function RegisterPage() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' }); 
  const [modalContent, setModalContent] = useState(null); 


  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  // confirmPassword state'e eklendi
  const [formData, setFormData] = useState({
    firstName: '', 
    lastName: '', 
    email: '', 
    password: '',
    confirmPassword: '', 
    isKvkkApproved: false,
    isTermsAccepted: false
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({ 
      ...formData, 
      [name]: type === 'checkbox' ? checked : value 
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    // 1. Şifre eşleşme kontrolü
    if (formData.password !== formData.confirmPassword) {
      setMessage({ type: 'error', text: 'Girdiğiniz şifreler eşleşmiyor. Lütfen kontrol edin.' });
      return;
    }

    // 2. KVKK ve Şartlar kontrolü
    if (!formData.isKvkkApproved || !formData.isTermsAccepted) {
      setMessage({ type: 'error', text: 'Kayıt olmak için KVKK ve Kullanım Şartlarını onaylamalısınız.' });
      return;
    }

    setLoading(true);
    setMessage({ type: '', text: '' });

    // Backend'e confirmPassword'ü göndermemek için payload'ı temizliyoruz
    const payload = {
      firstName: formData.firstName,
      lastName: formData.lastName,
      email: formData.email,
      password: formData.password,
      isKvkkApproved: formData.isKvkkApproved,
      isTermsAccepted: formData.isTermsAccepted
    };

    try {
      const response = await authService.register(payload);
      if (response.success) {
        setMessage({ type: 'success', text: 'Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz.' });
        setTimeout(() => navigate('/login'), 1500);
      } else {
        setMessage({ type: 'error', text: response.message || 'Kayıt başarısız.' });
      }
    } catch (err) {
      setMessage({ type: 'error', text: err.response?.data?.message || 'Kayıt sırasında bir hata oluştu.' });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="register-page">
      <div className="register-card">
        <div className="header">
          <div className="logo">📚</div>
          <h1 className="title">Kütüphane Sistemi</h1>
          <p className="subtitle">Yeni hesap oluşturun</p>
        </div>

        {message.text && (
          <div className={message.type === 'error' ? 'alert-error' : 'alert-success'}>
            <span>{message.type === 'error' ? '⚠️' : '✓'}</span>
            <span>{message.text}</span>
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <label className="label">Ad</label>
            <input className="input-field" name="firstName" type="text" placeholder="Örn: Ahmet" required onChange={handleChange} />
          </div>

          <div className="input-group">
            <label className="label">Soyad</label>
            <input className="input-field" name="lastName" type="text" placeholder="Örn: Yılmaz" required onChange={handleChange} />
          </div>

          <div className="input-group">
            <label className="label">E-posta</label>
            <input className="input-field" name="email" type="email" placeholder="ornek@email.com" required onChange={handleChange} />
          </div>

          {/* Şifre Alanı (Göz İkonlu) */}
          <div className="input-group">
            <label className="label">Şifre</label>
            <div className="password-wrapper">
              <input 
                className="input-field" 
                name="password" 
                type={showPassword ? "text" : "password"} 
                required 
                minLength={6} 
                onChange={handleChange} 
              />
              <button 
                type="button" 
                className="eye-icon" 
                onClick={() => setShowPassword(!showPassword)}
                title={showPassword ? "Şifreyi Gizle" : "Şifreyi Göster"}
              >
                {showPassword ? (
              /* GİZLE İKONU (Üstü Çizik Göz) */
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M9.88 9.88a3 3 0 1 0 4.24 4.24" />
                <path d="M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68" />
                <path d="M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61" />
                <line x1="2" y1="2" x2="22" y2="22" />
              </svg>
              ) : (
              /* GÖSTER İKONU (Açık Göz) */
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z" />
                <circle cx="12" cy="12" r="3" />
              </svg>
              )}
              </button>
            </div>
          </div>

        {/* Şifre Tekrar Alanı (Göz İkonlu) */}
          <div className="input-group">
            <label className="label">Şifre Tekrar</label>
            <div className="password-wrapper">
              <input 
                className="input-field" 
                name="confirmPassword" 
                type={showConfirmPassword ? "text" : "password"} 
                required 
                minLength={6} 
                onChange={handleChange} 
              />
              <button 
                type="button" 
                className="eye-icon" 
                onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                title={showConfirmPassword ? "Şifreyi Gizle" : "Şifreyi Göster"}
              >
               {showConfirmPassword ? (
              /* GİZLE İKONU (Üstü Çizik Göz) */
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M9.88 9.88a3 3 0 1 0 4.24 4.24" />
                <path d="M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68" />
                <path d="M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61" />
                <line x1="2" y1="2" x2="22" y2="22" />
              </svg>
            ) : (
              /* GÖSTER İKONU (Açık Göz) */
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z" />
                <circle cx="12" cy="12" r="3" />
              </svg>
            )}
              </button>
            </div>
          </div>

          <div className="checkbox-group">
            <input 
              type="checkbox" 
              name="isKvkkApproved" 
              id="isKvkkApproved" 
              checked={formData.isKvkkApproved} 
              onChange={handleChange} 
              required 
            />
            <label htmlFor="isKvkkApproved">
              <span className="clickable-text" onClick={() => setModalContent('kvkk')}>KVKK Aydınlatma Metnini</span> okudum ve onaylıyorum.
            </label>
          </div>

          <div className="checkbox-group" style={{ marginBottom: '20px' }}>
            <input 
              type="checkbox" 
              name="isTermsAccepted" 
              id="isTermsAccepted" 
              checked={formData.isTermsAccepted} 
              onChange={handleChange} 
              required 
            />
            <label htmlFor="isTermsAccepted">
              <span className="clickable-text" onClick={() => setModalContent('terms')}>Kullanım Şartlarını</span> kabul ediyorum.
            </label>
          </div>

          <button className="submit-btn" type="submit" disabled={loading}>
            {loading ? 'Bekleyin...' : 'Kayıt Ol'}
          </button>
        </form>

        <div className="switch-container">
          <span>Zaten hesabınız var mı?</span>
          <Link to="/login" className="switch-link">
            Giriş Yap
          </Link>
        </div>
      </div>

   {/* ŞIK POP-UP (MODAL) ALANI */}
      {modalContent && (
        <div className="modal-overlay" onClick={() => setModalContent(null)}>
          {/* İçeriğe tıklayınca kapanmasını engellemek için e.stopPropagation() kullanıyoruz */}
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            
            <div className="modal-header">
              <h3>{modalContent === 'kvkk' ? 'KVKK Aydınlatma Metni' : 'Kullanım Şartları'}</h3>
              <button className="close-icon" onClick={() => setModalContent(null)}>×</button>
            </div>

            <div className="modal-body">
              {modalContent === 'kvkk' ? (
                <>
                  <p><strong>1. Veri Sorumlusunun Kimliği</strong></p>
                  <p>Lumina Kütüphane Yönetim Sistemi olarak kişisel verilerinizin güvenliğine büyük önem veriyoruz. 6698 sayılı Kişisel Verilerin Korunması Kanunu ("KVKK") uyarınca, üyelik oluşturma sürecinde paylaştığınız veriler güvence altındadır.</p>
                  
                  <p><strong>2. İşlenen Kişisel Veriler ve İşlenme Amacı</strong></p>
                  <p>Ad, soyad ve e-posta bilgileriniz yalnızca kütüphane otomasyon hizmetlerinin yürütülmesi, yetkilendirme, kitap ödünç alma süreçlerinin takibi ve güvenlik amaçlarıyla işlenmektedir. Hiçbir ticari amaçla üçüncü şahıslarla paylaşılmaz.</p>
                  
                  <p><strong>3. Veri Güvenliği</strong></p>
                  <p>Sistemimizde şifreleriniz tek yönlü ve güçlü kriptografik algoritmalarla (HMACSHA512) geri döndürülemez şekilde şifrelenerek saklanmaktadır.</p>
                </>
              ) : (
                <>
                  <p><strong>Genel Kurallar</strong></p>
                  <p>Bu kütüphane sistemini kullanarak kütüphane materyallerini korumayı, verilen kurallara uymayı ve hesap güvenliğinizi sağlamayı taahhüt etmiş olursunuz.</p>
                  
                  <p><strong>Hesap Güvenliği</strong></p>
                  <p>Hesap bilgilerinizin gizliliğinden siz sorumlusunuz. Şüpheli bir durumda sistem yöneticileri hesabı askıya alma hakkına sahiptir.</p>
                  
                  <p><strong>Materyal Kullanımı</strong></p>
                  <p>Ödünç alınan kitapların süresinde ve hasarsız teslim edilmesi esastır. Gecikme veya hasar durumlarında sistemin belirlediği ceza politikaları uygulanır.</p>
                </>
              )}
            </div>

            <div className="modal-footer">
              <button className="accept-modal-btn" onClick={() => setModalContent(null)}>
                Okudum, Anladım
              </button>
            </div>

          </div>
        </div>
      )}
    </div>
  );
}