import { PERMISSIONS } from '../../auth/permissionKeys';

export const menuItems = [

  {
    title: 'Katalog',
    path: '/catalog',
    icon: '🔍',
    // requiredPermission yok, giriş yapan herkes görür
  },
  {
    title: 'Üyelik Durumum',
    path: '/membership-apply',
    icon: '📄',
  },


  {
    title: 'Dashboard',
    path: '/dashboard',
    icon: '📊',
    requiredPermission: PERMISSIONS.VIEW_DASHBOARD
  },
  {
    title: 'Üyeler',
    path: '/members',
    icon: '👥',
    requiredPermission: PERMISSIONS.MANAGE_MEMBERS,
  },
  {
    title: 'Başvurular',
    path: '/applications',
    icon: '📋',
    requiredPermission: PERMISSIONS.MANAGE_MEMBERS
  },
  {
    title: 'Ödünçler',
    path: '/loans',
    icon: '📖',
    requiredPermission: PERMISSIONS.MANAGE_LOANS
  },

  { 
    title: 'Lost Books', 
    path: '/lost-books', 
    icon: '🚨', 
    requiredPermission: PERMISSIONS.MANAGE_LOANS 
  },

  {
    title: 'Rezervasyonlar',
    path: '/reservations',
    icon: '⏳',
    requiredPermission: PERMISSIONS.MANAGE_LOANS
  },


{
    title: 'Rezervasyonlarım',
    path: '/my-reservations',
    icon: '⏳',
    requiredPermission: PERMISSIONS.VIEW_MY_RESERVATIONS,
  },
  {
    title: 'Ödünç Aldıklarım',
    path: '/my-loans',
    icon: '📚',
    requiredPermission: PERMISSIONS.VIEW_MY_LOANS,
  },
  {
    title: 'Cezalarım',
    path: '/my-penalties',
    icon: '⚠️',
    requiredPermission: PERMISSIONS.VIEW_MY_PENALTIES,
  },

];
