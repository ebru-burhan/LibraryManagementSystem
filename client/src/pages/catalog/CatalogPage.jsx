// src/pages/catalog/CatalogPage.jsx
import React from 'react';
import { useNavigate } from 'react-router-dom';

const CatalogPage = () => {
    const navigate = useNavigate();

    return (
        <div style={{ padding: '40px', textAlign: 'center', fontFamily: 'sans-serif' }}>
            <h2>Katalog Yönetimi</h2>
            <p style={{ color: '#666', marginBottom: '20px' }}>Katalog işlemlerini bu sayfadan yönetebilirsiniz.</p>
            
            <button 
                onClick={() => navigate('/book-copies/add')}
                style={{
                    backgroundColor: '#e07a2a',
                    color: 'white',
                    border: 'none',
                    padding: '12px 24px',
                    borderRadius: '6px',
                    cursor: 'pointer',
                    fontWeight: '600',
                    fontSize: '14px',
                    boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
                }}
            >
                + Yeni Fiziksel Kopya Ekle
            </button>
        </div>
    );
};

export default CatalogPage;