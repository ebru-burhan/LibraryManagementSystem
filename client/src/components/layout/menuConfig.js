// Hangi menünün görünmesi için hangi yetkinin gerektiğini burada tanımlıyoruz
export const menuItems = [
  { 
    title: 'Dashboard', 
    path: '/dashboard', 
    icon: '📊', 
    requiredPermission: 'view_dashboard' // Admin veritabanından bu kutucuğu işaretlemeli
  },

  { 
    title: 'Members', 
    path: '/members', 
    icon: '👥', 
    requiredPermission: 'manage_members' 
  },
  { 
    title: 'My Loans', 
    path: '/my-loans', 
    icon: '📖', 
    requiredPermission: 'view_loans' 
  },
  { 
    title: 'Catalog', 
    path: '/catalog', 
    icon: '🔍', 
    requiredPermission: null // null demek: Sisteme giriş yapan herkes görebilir
  }
];