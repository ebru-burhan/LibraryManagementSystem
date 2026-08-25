import React, { useState } from 'react';
import { authService } from '../services/api';
import { useNavigate } from 'react-router-dom';
import './LoginPage.css'; // CSS dosyamızı buraya import ettik

export default function LoginPage() {
  const navigate = useNavigate();
  const [isRegister, setIsRegister] = useState(false);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' }); 

  // Form State
  const [formData, setFormData] = useState({
    firstName: '', lastName: '', identityNumber: '', 
    phoneNumber: '', address: '', email: '', password: ''
  });

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage({ type: '', text: '' });

    try {
      if (isRegister) {
        const response = await authService.register(formData);
        if (response.success) {
          setMessage({ type: 'success', text: 'Kayıt başarılı! Giriş yapabilirsiniz.' });
          setTimeout(() => setIsRegister(false), 1500);
        } else {
          setMessage({ type: 'error', text: response.message || 'Kayıt başarısız.' });
        }
      } else {
        const response = await authService.login({ email: formData.email, password: formData.password });
        if (response.success && response.data?.token) {
          localStorage.setItem('token', response.data.token);
          navigate('/dashboard');
        } else {
          setMessage({ type: 'error', text: 'E-posta veya şifre hatalı.' });
        }
      }
    } catch (err) {
      setMessage({ type: 'error', text: 'İşlem sırasında bir hata oluştu.' });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="header">
          <div className="logo">📚</div>
          <h1 className="title">Kütüphane Sistemi</h1>
          <p className="subtitle">{isRegister ? 'Yeni hesap oluşturun' : 'Giriş yapın'}</p>
        </div>

        {message.text && (
          <div className={message.type === 'error' ? 'alert-error' : 'alert-success'}>
            <span>{message.type === 'error' ? '⚠️' : '✓'}</span>
            <span>{message.text}</span>
          </div>
        )}

        <form onSubmit={handleSubmit}>
          {isRegister && (
            <>
              <div className="input-group">
                <label className="label">Ad</label>
                <input className="input-field" name="firstName" type="text" required onChange={handleChange} />
              </div>
              <div className="input-group">
                <label className="label">Soyad</label>
                <input className="input-field" name="lastName" type="text" required onChange={handleChange} />
              </div>
              <div className="input-group">
                <label className="label">T.C. Kimlik No</label>
                <input className="input-field" name="identityNumber" type="text" required maxLength={11} onChange={handleChange} />
              </div>
              <div className="input-group">
                <label className="label">Telefon (İsteğe bağlı)</label>
                <input className="input-field" name="phoneNumber" type="tel" onChange={handleChange} />
              </div>
              <div className="input-group">
                <label className="label">Adres (İsteğe bağlı)</label>
                <input className="input-field" name="address" type="text" onChange={handleChange} />
              </div>
            </>
          )}

          <div className="input-group">
            <label className="label">E-posta</label>
            <input className="input-field" name="email" type="email" required onChange={handleChange} />
          </div>
          <div className="input-group">
            <label className="label">Şifre</label>
            <input className="input-field" name="password" type="password" required minLength={6} onChange={handleChange} />
          </div>

          <button className="submit-btn" type="submit" disabled={loading}>
            {loading ? 'Bekleyin...' : (isRegister ? 'Kayıt Ol' : 'Giriş Yap')}
          </button>
        </form>

        <div className="switch-container">
          <span>{isRegister ? 'Zaten hesabınız var mı?' : 'Hesabınız yok mu?'}</span>
          <button type="button" className="switch-btn" onClick={() => setIsRegister(!isRegister)}>
            {isRegister ? 'Giriş Yap' : 'Kayıt Ol'}
          </button>
        </div>
      </div>
    </div>
  );
}