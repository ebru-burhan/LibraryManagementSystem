import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth'; 
import { menuItems } from './menuConfig'; 
import './Sidebar.css';

export default function Sidebar() {
  const navigate = useNavigate();
  // Artık rollerle ve portallarla işimiz yok, sadece yetkileri (permissions) alıyoruz!
  const { permissions } = useAuth(); 

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  // FİLTRE: Menüleri tamamen kullanıcının cebindeki yetkilere göre süzüyoruz
  const filteredMenus = menuItems.filter(item => {
    if (!item.requiredPermission) return true;
    return permissions.includes(item.requiredPermission);
  });

  return (
    <aside className="sidebar">
      {/* Sabit, tertemiz logo alanı */}
      <div className="sidebar-header">
        <div className="logo-icon">📚</div>
        <div className="logo-text">
          <h2>Lumina Library</h2>
          <span> Portal</span> {/* İstersen burayı da sabit bir alt yazı yapabiliriz */}
        </div>
      </div>

      {/* "+ New Entry" butonu da artık rol adına değil, doğrudan yetkiye bağlı! */}
      {permissions.includes('create_book') && (
        <div className="sidebar-action" style={{ marginBottom: '24px' }}>
          <button className="new-entry-btn">+ New Entry</button>
        </div>
      )}

      {/* Dinamik Menü Alanı */}
      <nav className="sidebar-nav">
        {filteredMenus.map((menu) => (
          <NavLink key={menu.path} to={menu.path} className="nav-item">
            <span className="nav-icon">{menu.icon}</span>
            {menu.title}
          </NavLink>
        ))}
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