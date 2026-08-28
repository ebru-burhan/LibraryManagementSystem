import React, { useState } from 'react';
import { membershipService } from '../../services/api';

export default function MembershipApplicationPage() {
  const [formData, setFormData] = useState({
    identityNumber: '',
    dateOfBirth: '',
    phoneNumber: '',
    address: '',
    isKvkkApproved: false,
    isTermsAccepted: false,
  });

  const [message, setMessage] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setMessage(null);

    try {
      const response = await membershipService.apply(formData);
      setMessage(response.message || "Başvurunuz başarıyla alındı ve onay sürecine girdi.");
    } catch (err) {
      setError(err.response?.data?.message || "Başvuru sırasında bir hata oluştu.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="application-container" style={{ padding: '20px', maxWidth: '600px', margin: '0 auto' }}>
      <h2>Üyelik Başvuru Formu</h2>
      {message && <div style={{ color: 'green', marginBottom: '15px' }}>{message}</div>}
      {error && <div style={{ color: 'red', marginBottom: '15px' }}>{error}</div>}

      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
        <div>
          <label>T.C. Kimlik Numarası: </label>
          <input type="text" name="identityNumber" value={formData.identityNumber} onChange={handleChange} required />
        </div>
        <div>
          <label>Doğum Tarihi: </label>
          <input type="date" name="dateOfBirth" value={formData.dateOfBirth} onChange={handleChange} required />
        </div>
        <div>
          <label>Telefon Numarası: </label>
          <input type="text" name="phoneNumber" value={formData.phoneNumber} onChange={handleChange} required />
        </div>
        <div>
          <label>Adres: </label>
          <textarea name="address" value={formData.address} onChange={handleChange} />
        </div>
        <div>
          <label>
            <input type="checkbox" name="isKvkkApproved" checked={formData.isKvkkApproved} onChange={handleChange} />
            KVKK Metnini Okudum ve Onaylıyorum
          </label>
        </div>
        <div>
          <label>
            <input type="checkbox" name="isTermsAccepted" checked={formData.isTermsAccepted} onChange={handleChange} />
            Kullanım Şartlarını Kabul Ediyorum
          </label>
        </div>
        
        <button type="submit" disabled={loading} style={{ padding: '10px', cursor: 'pointer' }}>
          {loading ? 'Gönderiliyor...' : 'Başvuruyu Gönder'}
        </button>
      </form>
    </div>
  );
}