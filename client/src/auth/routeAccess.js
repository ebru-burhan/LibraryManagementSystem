import { PERMISSIONS } from './permissionKeys';

// Menü dosyası UI metnini tutar; hangi path'in hangi JWT yetkisiyle görüneceği burada tutulur.
export const pathPermissions = {
  '/dashboard': PERMISSIONS.VIEW_DASHBOARD,
  '/members': PERMISSIONS.MANAGE_MEMBERS,
  '/applications': PERMISSIONS.MANAGE_MEMBERS,
  '/reservations': PERMISSIONS.MANAGE_LOANS,

  '/loans': PERMISSIONS.MANAGE_LOANS,
  '/lost-books': PERMISSIONS.MANAGE_LOANS,
  

  '/book-copies/add': PERMISSIONS.MANAGE_CATALOG,
  
  // Üye (Member) Paneli Rotaları
  '/my-reservations': PERMISSIONS.VIEW_MY_RESERVATIONS,
  '/my-loans': PERMISSIONS.VIEW_MY_LOANS,
  '/my-penalties': PERMISSIONS.VIEW_MY_PENALTIES,
};