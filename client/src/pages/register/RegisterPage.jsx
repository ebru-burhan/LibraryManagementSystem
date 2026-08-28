import React, { useState } from 'react';
import { authService } from '../../services/api';
import { useNavigate, Link } from 'react-router-dom';
import './RegisterPage.css'; 

export default function RegisterPage() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' }); 

  // Form State Kapsülleme
  const [formData, setFormData] = useState({
    firstName: '', 
    lastName: '', 
    identityNumber: '', 
    phoneNumber: '', 
    address: '', 
    email: '', 
    password: ''
  });

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage({ type: '', text: '' });

    try {
      const response = await authService.register(formData);
      if (response.success) {
        setMessage({ type: 'success', text: 'Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz.' });
        setTimeout(() => navigate('/login'), 1500);
      } else {
        setMessage({ type: 'error', text: response.message || 'Kayıt başarısız.' });
      }
    } catch (err) {
      setMessage({ type: 'error', text: 'Kayıt sırasında bir hata oluştu.' });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="register-page">
      <div className="register-card">
        <div className="header">
          <div className="logo">📚</div>
          <h1 className="title">Lumina Library</h1>
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
            <label className="label">Telefon</label>
            <input className="input-field" name="phoneNumber" type="tel" required onChange={handleChange} />
          </div>

          <div className="input-group">
            <label className="label">Adres</label>
            <input className="input-field" name="address" type="text" onChange={handleChange} />
          </div>

          <div className="input-group">
            <label className="label">E-posta</label>
            <input className="input-field" name="email" type="email" required onChange={handleChange} />
          </div>

          <div className="input-group">
            <label className="label">Şifre</label>
            <input className="input-field" name="password" type="password" required minLength={6} onChange={handleChange} />
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
    </div>
  );
}