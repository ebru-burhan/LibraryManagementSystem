import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth'; // Zeka birimimizi içeri alıyoruz
import './Sidebar.css';

export default function Sidebar() {
  const navigate = useNavigate();
  
  // Hook'umuzu çağırıp kullanıcının yetkilerini alıyoruz
  const { isAdmin, isMember } = useAuth(); 

  const handleLogout = () => {
    //kulanıcı çıkınca tokenı sil ve logine şutla
    localStorage.removeItem('token');
    navigate('/');
  };

  return (
    <aside className="sidebar">
      {/* Logo Alanı */}
      <div className="sidebar-header">
        <div className="logo-icon">📚</div>
        <div className="logo-text">
          <h2>Lumina Library</h2>
          {/* Alt başlık bile role göre değişiyor! */}
          <span>{isAdmin ? 'Admin Portal' : 'User Portal'}</span>
        </div>
      </div>

      {/* Ana Eylem Butonu - Sadece Admin yeni kayıt yapabilir */}
      {isAdmin && (
        <div className="sidebar-action">
          <button className="new-entry-btn">+ New Entry</button>
        </div>
      )}

      {/* Menü Linkleri */}
      <nav className="sidebar-nav">
        
        {/* 1. SADECE ADMİNLERİN GÖRECEĞİ MENÜLER */}
        {isAdmin && (
          <>
            <NavLink to="/dashboard" className="nav-item">
              <span className="nav-icon">📊</span>
              Dashboard
            </NavLink>
            
            <NavLink to="/applications" className="nav-item">
              <span className="nav-icon">📂</span>
              Applications
            </NavLink>

            <NavLink to="/members" className="nav-item">
              <span className="nav-icon">👥</span>
              Members
            </NavLink>
          </>
        )}

        {/* 2. SADECE ONAYLI ÜYELERİN (MEMBER) GÖRECEĞİ MENÜLER */}
        {isMember && (
          <NavLink to="/my-loans" className="nav-item">
            <span className="nav-icon">📖</span>
            My Loans
          </NavLink>
        )}

        {/* 3. HERKESİN (Admin, Member, Düz User) GÖRECEĞİ ORTAK MENÜ */}
        <NavLink to="/catalog" className="nav-item">
          <span className="nav-icon">🔍</span>
          Catalog
        </NavLink>
      </nav>

      {/* Alt Kısım (Çıkış) */}
      <div className="sidebar-footer">
        <button onClick={handleLogout} className="logout-btn">
          <span className="nav-icon">🚪</span>
          Sign Out
        </button>
      </div>
    </aside>
  );
}