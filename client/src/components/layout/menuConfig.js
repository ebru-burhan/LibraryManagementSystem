export const menuItems = [
  {
    title: 'Dashboard',
    path: '/dashboard',
    icon: '📊',
  },
  {
    title: 'Üyeler',
    path: '/members',
    icon: '👥',
  },
  {
    title: 'Başvurular',
    path: '/applications',
    icon: '📋',
  },
  {
    title: 'Ödünçler',
    path: '/loans',
    icon: '📖',
  },
  {
    title: 'Katalog',
    path: '/catalog',
    icon: '🔍',
  },
  { 
    title: 'Lost Books', 
    path: '/lost-books', 
    icon: '🚨', 
    requiredPermission: 'view_loans' 
  },
];
