import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth'; 
import { menuItems } from './menuConfig';
import { pathPermissions } from '../../auth/routeAccess';
import { PERMISSIONS } from '../../auth/permissionKeys';
import './Sidebar.css';

export default function Sidebar() {
  const navigate = useNavigate();
  const { permissions } = useAuth(); 

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  const filteredMenus = menuItems.filter((item) => {
    const requiredPermission = pathPermissions[item.path];
    if (!requiredPermission) return true;
    return permissions.includes(requiredPermission);
  });

  return (
    <aside className="sidebar">
      <div className="sidebar-header">
        <div className="logo-icon">📚</div>
        <div className="logo-text">
          <h2>Lumina Library</h2>
          <span> Portal</span>
        </div>
      </div>

      {permissions.includes(PERMISSIONS.CREATE_BOOK) && (
        <div className="sidebar-action" style={{ marginBottom: '24px' }}>
          <button className="new-entry-btn">+ New Entry</button>
        </div>
      )}

      <nav className="sidebar-nav">
        {filteredMenus.map((menu) => (
          <NavLink key={menu.path} to={menu.path} className="nav-item">
            <span className="nav-icon">{menu.icon}</span>
            {menu.title}
          </NavLink>
        ))}
      </nav>

      <div className="sidebar-footer">
        <button onClick={handleLogout} className="logout-btn">
          <span className="nav-icon">🚪</span>
          Sign Out
        </button>
      </div>
    </aside>
  );
}
