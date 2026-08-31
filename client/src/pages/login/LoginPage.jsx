import React, { useState } from 'react';
import { authService } from '../../services/api';
import { useNavigate, Link } from 'react-router-dom';
import './LoginPage.css'; 

export default function LoginPage() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' }); 

  const [showPassword, setShowPassword] = useState(false);

  // Form State Kapsülleme
  const [formData, setFormData] = useState({
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
      const response = await authService.login({ email: formData.email, password: formData.password });
      if (response.success && response.data?.token) {
        localStorage.setItem('token', response.data.token);
        navigate('/dashboard');
      } else {
        setMessage({ type: 'error', text: response.message || 'E-posta veya şifre hatalı.' });
      }
    } catch (err) {
      setMessage({ type: 'error', text: 'Giriş yapılırken bir hata oluştu.' });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="header">
          <div className="logo">📚</div>
          <h1 className="title">Lumina Library</h1>
          <p className="subtitle">Giriş yapın</p>
        </div>

        {message.text && (
          <div className={message.type === 'error' ? 'alert-error' : 'alert-success'}>
            <span>{message.type === 'error' ? '⚠️' : '✓'}</span>
            <span>{message.text}</span>
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <label className="label">E-posta</label>
            <input className="input-field" name="email" type="email" required onChange={handleChange} />
          </div>

          <div className="input-group">
            <label className="label">Şifre</label>
            <div className="password-wrapper">
              <input 
                className="input-field" 
                name="password" 
                type={showPassword ? "text" : "password"} 
                required 
                onChange={handleChange} 
              />
              <button 
                type="button" 
                className="eye-icon" 
                onClick={() => setShowPassword(!showPassword)}
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

          <button className="submit-btn" type="submit" disabled={loading}>
            {loading ? 'Bekleyin...' : 'Giriş Yap'}
          </button>
        </form>

        <div className="switch-container">
          <span>Hesabınız yok mu?</span>
          <Link to="/register" className="switch-link">
            Kayıt Ol
          </Link>
        </div>
      </div>
    </div>
  );
}