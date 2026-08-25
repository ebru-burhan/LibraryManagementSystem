import React, { useState } from 'react';
import { authService } from '../services/api';
import { useNavigate } from 'react-router-dom';

export default function LoginPage() {
  const navigate = useNavigate();

  // Login / Register geçişi
  const [isRegister, setIsRegister] = useState(false);

  // Register alanları
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [identityNumber, setIdentityNumber] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [address, setAddress] = useState('');

  // Ortak alanlar
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  // UI
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [loading, setLoading] = useState(false);

  // =====================================================
  // HATA MESAJINI OKU
  // =====================================================

  const getErrorMessage = (err) => {
    const data = err.response?.data;

    // Backend'den gelen klasik message
    if (data?.message) {
      return data.message;
    }

    // ASP.NET Core validation hataları
    if (data?.errors) {
      const errors = data.errors;

      const messages = Object.entries(errors)
        .flatMap(([field, fieldErrors]) => {
          if (Array.isArray(fieldErrors)) {
            return fieldErrors.map((message) => {
              return `${getFieldName(field)}: ${message}`;
            });
          }

          return [`${getFieldName(field)}: ${fieldErrors}`];
        });

      if (messages.length > 0) {
        return messages.join(' | ');
      }
    }

    // Axios error
    if (err.message) {
      return err.message;
    }

    return 'İşlem sırasında bir hata oluştu.';
  };

  // =====================================================
  // BACKEND FIELD İSİMLERİNİ TÜRKÇELEŞTİR
  // =====================================================

  const getFieldName = (field) => {
    const fieldNames = {
      FirstName: 'Ad',
      LastName: 'Soyad',
      IdentityNumber: 'T.C. Kimlik No',
      PhoneNumber: 'Telefon',
      Address: 'Adres',
      Email: 'E-posta',
      Password: 'Şifre',
    };

    return fieldNames[field] || field;
  };

  // =====================================================
  // FORM SUBMIT
  // =====================================================

  const handleSubmit = async (e) => {
    e.preventDefault();

    setErrorMessage('');
    setSuccessMessage('');
    setLoading(true);

    try {
      // =================================================
      // REGISTER
      // =================================================

      if (isRegister) {
        const registerDto = {
          firstName: firstName.trim(),
          lastName: lastName.trim(),
          identityNumber: identityNumber.trim(),
          phoneNumber: phoneNumber.trim(),
          address: address.trim(),
          email: email.trim(),
          password: password,
        };

        console.log('REGISTER DTO:', registerDto);

        const response = await authService.register(registerDto);

        if (response.success) {
          setSuccessMessage(
            response.message ||
              'Kayıt başarılı! Şimdi giriş yapabilirsiniz.'
          );

          // Formu temizle
          setFirstName('');
          setLastName('');
          setIdentityNumber('');
          setPhoneNumber('');
          setAddress('');
          setEmail('');
          setPassword('');

          // 1.5 saniye sonra login ekranına geç
          setTimeout(() => {
            setIsRegister(false);
            setSuccessMessage('');
          }, 1500);
        } else {
          setErrorMessage(
            response.message || 'Kayıt işlemi başarısız.'
          );
        }

        return;
      }

      // =================================================
      // LOGIN
      // =================================================

      const loginDto = {
        email: email.trim(),
        password: password,
      };

      console.log('LOGIN DTO:', loginDto);

      const response = await authService.login(loginDto);

      if (!response.success) {
        setErrorMessage(
          response.message || 'E-posta veya şifre hatalı.'
        );

        return;
      }

      // =================================================
      // TOKEN
      // =================================================

      const token = response.data?.token;

      if (!token) {
        console.error('Backend response:', response);

        throw new Error(
          'Giriş başarılı ancak backend token göndermedi.'
        );
      }

      // JWT token
      localStorage.setItem('token', token);

      // Kullanıcı bilgisi varsa kaydet
      if (response.data?.user) {
        localStorage.setItem(
          'user',
          JSON.stringify(response.data.user)
        );
      }

      // Dashboard'a git
      navigate('/dashboard');

    } catch (err) {
      console.error('AUTH ERROR:', err);

      setErrorMessage(getErrorMessage(err));

    } finally {
      setLoading(false);
    }
  };

  // =====================================================
  // LOGIN / REGISTER DEĞİŞTİR
  // =====================================================

  const handleModeChange = () => {
    setIsRegister((prev) => !prev);

    setErrorMessage('');
    setSuccessMessage('');

    setFirstName('');
    setLastName('');
    setIdentityNumber('');
    setPhoneNumber('');
    setAddress('');
    setEmail('');
    setPassword('');
  };

  // =====================================================
  // RENDER
  // =====================================================

  return (
    <div style={styles.page}>

      <div style={styles.card}>

        {/* =============================================
            HEADER
        ============================================== */}

        <div style={styles.header}>

          <div style={styles.logo}>
            📚
          </div>

          <h1 style={styles.title}>
            Kütüphane Sistemi
          </h1>

          <p style={styles.subtitle}>
            {isRegister
              ? 'Yeni hesabınızı oluşturun'
              : 'Hesabınıza giriş yapın'}
          </p>

        </div>

        {/* =============================================
            ERROR
        ============================================== */}

        {errorMessage && (
          <div style={styles.error}>
            <span style={styles.messageIcon}>
              ⚠️
            </span>

            <span>
              {errorMessage}
            </span>
          </div>
        )}

        {/* =============================================
            SUCCESS
        ============================================== */}

        {successMessage && (
          <div style={styles.success}>
            <span style={styles.messageIcon}>
              ✓
            </span>

            <span>
              {successMessage}
            </span>
          </div>
        )}

        {/* =============================================
            FORM
        ============================================== */}

        <form onSubmit={handleSubmit}>

          {/* ===========================================
              REGISTER ALANLARI
          ============================================ */}

          {isRegister && (
            <>
              {/* AD */}
              <div style={styles.inputGroup}>

                <label style={styles.label}>
                  Ad
                </label>

                <input
                  type="text"
                  placeholder="Adınız"
                  value={firstName}
                  onChange={(e) =>
                    setFirstName(e.target.value)
                  }
                  required
                  autoComplete="given-name"
                  style={styles.input}
                />

              </div>

              {/* SOYAD */}
              <div style={styles.inputGroup}>

                <label style={styles.label}>
                  Soyad
                </label>

                <input
                  type="text"
                  placeholder="Soyadınız"
                  value={lastName}
                  onChange={(e) =>
                    setLastName(e.target.value)
                  }
                  required
                  autoComplete="family-name"
                  style={styles.input}
                />

              </div>

              {/* TC */}
              <div style={styles.inputGroup}>

                <label style={styles.label}>
                  T.C. Kimlik No
                </label>

                <input
                  type="text"
                  placeholder="11 haneli T.C. Kimlik No"
                  value={identityNumber}
                  onChange={(e) =>
                    setIdentityNumber(
                      e.target.value.replace(/\D/g, '')
                    )
                  }
                  required
                  maxLength={11}
                  inputMode="numeric"
                  style={styles.input}
                />

              </div>

              {/* TELEFON */}
              <div style={styles.inputGroup}>

                <label style={styles.label}>
                  Telefon
                </label>

                <input
                  type="tel"
                  placeholder="05XX XXX XX XX"
                  value={phoneNumber}
                  onChange={(e) =>
                    setPhoneNumber(e.target.value)
                  }
                  required
                  autoComplete="tel"
                  style={styles.input}
                />

              </div>

              {/* ADRES */}
              <div style={styles.inputGroup}>

                <label style={styles.label}>
                  Adres
                </label>

                <textarea
                  placeholder="Adresinizi giriniz"
                  value={address}
                  onChange={(e) =>
                    setAddress(e.target.value)
                  }
                  required
                  rows={3}
                  style={styles.textarea}
                />

              </div>
            </>
          )}

          {/* ===========================================
              EMAIL
          ============================================ */}

          <div style={styles.inputGroup}>

            <label style={styles.label}>
              E-posta
            </label>

            <input
              type="email"
              placeholder="ornek@email.com"
              value={email}
              onChange={(e) =>
                setEmail(e.target.value)
              }
              required
              autoComplete="email"
              style={styles.input}
            />

          </div>

          {/* ===========================================
              PASSWORD
          ============================================ */}

          <div style={styles.inputGroup}>

            <label style={styles.label}>
              Şifre
            </label>

            <input
              type="password"
              placeholder="En az 6 karakter"
              value={password}
              onChange={(e) =>
                setPassword(e.target.value)
              }
              required
              minLength={6}
              autoComplete={
                isRegister
                  ? 'new-password'
                  : 'current-password'
              }
              style={styles.input}
            />

          </div>

          {/* ===========================================
              SUBMIT
          ============================================ */}

          <button
            type="submit"
            disabled={loading}
            style={{
              ...styles.button,
              opacity: loading ? 0.7 : 1,
              cursor: loading
                ? 'not-allowed'
                : 'pointer',
            }}
          >
            {loading
              ? 'Lütfen bekleyin...'
              : isRegister
                ? 'Kayıt Ol'
                : 'Giriş Yap'}
          </button>

        </form>

        {/* =============================================
            LOGIN / REGISTER SWITCH
        ============================================== */}

        <div style={styles.switchContainer}>

          <span>
            {isRegister
              ? 'Zaten hesabınız var mı?'
              : 'Hesabınız yok mu?'}
          </span>

          <button
            type="button"
            onClick={handleModeChange}
            style={styles.switchButton}
          >
            {isRegister
              ? 'Giriş Yap'
              : 'Kayıt Ol'}
          </button>

        </div>

      </div>

    </div>
  );
}

// =====================================================
// STYLES
// =====================================================

const styles = {

  page: {
    minHeight: '100vh',
    width: '100%',

    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',

    background:
      'linear-gradient(135deg, #fff7ed 0%, #ffedd5 50%, #fed7aa 100%)',

    padding: '20px',

    boxSizing: 'border-box',
  },

  card: {
    width: '100%',
    maxWidth: '460px',

    backgroundColor: '#ffffff',

    borderRadius: '20px',

    padding: '40px',

    boxSizing: 'border-box',

    boxShadow:
      '0 20px 50px rgba(124, 45, 18, 0.15)',
  },

  header: {
    textAlign: 'center',
    marginBottom: '30px',
  },

  logo: {
    width: '70px',
    height: '70px',

    margin: '0 auto 15px',

    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',

    borderRadius: '18px',

    backgroundColor: '#fff7ed',

    fontSize: '38px',
  },

  title: {
    margin: '0',

    fontSize: '27px',

    fontWeight: '700',

    color: '#292524',
  },

  subtitle: {
    marginTop: '8px',
    marginBottom: '0',

    color: '#78716c',

    fontSize: '14px',
  },

  inputGroup: {
    marginBottom: '17px',
  },

  label: {
    display: 'block',

    marginBottom: '7px',

    fontSize: '14px',

    fontWeight: '600',

    color: '#44403c',
  },

  input: {
    width: '100%',

    boxSizing: 'border-box',

    padding: '13px 14px',

    border: '1px solid #d6d3d1',

    borderRadius: '10px',

    fontSize: '15px',

    color: '#292524',

    backgroundColor: '#ffffff',

    outline: 'none',
  },

  textarea: {
    width: '100%',

    boxSizing: 'border-box',

    padding: '13px 14px',

    border: '1px solid #d6d3d1',

    borderRadius: '10px',

    fontSize: '15px',

    color: '#292524',

    backgroundColor: '#ffffff',

    outline: 'none',

    resize: 'vertical',

    fontFamily: 'inherit',
  },

  button: {
    width: '100%',

    padding: '14px',

    marginTop: '5px',

    border: 'none',

    borderRadius: '10px',

    backgroundColor: '#f97316',

    color: '#ffffff',

    fontSize: '16px',

    fontWeight: '700',

    transition: '0.2s',
  },

  error: {
    display: 'flex',

    gap: '8px',

    alignItems: 'flex-start',

    backgroundColor: '#fef2f2',

    color: '#b91c1c',

    padding: '12px 14px',

    borderRadius: '10px',

    marginBottom: '20px',

    fontSize: '14px',

    lineHeight: '1.5',
  },

  success: {
    display: 'flex',

    gap: '8px',

    alignItems: 'center',

    backgroundColor: '#f0fdf4',

    color: '#15803d',

    padding: '12px 14px',

    borderRadius: '10px',

    marginBottom: '20px',

    fontSize: '14px',
  },

  messageIcon: {
    flexShrink: 0,
  },

  switchContainer: {
    marginTop: '25px',

    textAlign: 'center',

    fontSize: '14px',

    color: '#78716c',
  },

  switchButton: {
    background: 'none',

    border: 'none',

    color: '#ea580c',

    fontWeight: '700',

    cursor: 'pointer',

    marginLeft: '5px',

    fontSize: '14px',
  },
};