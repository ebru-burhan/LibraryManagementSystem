import React from 'react';
import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar'; 
import './AdminLayout.css';

export default function AdminLayout() {
  return (
    <div className="admin-layout"  style={{ display: 'flex', height: '100vh', backgroundColor: 'var(--bg-color)' }}>
      {/* Sol Menü (Sayfa değişse de sabit kalacak) */}
      <Sidebar />
      
      {/* Sağ İçerik Alanı (Tıklanan menüye göre dinamik değişecek) */}
      <div className="main-content"  style={{ flex: 1, padding: '40px', overflowY: 'auto' }}>
        <Outlet /> 
      </div>
    </div>
  );
}