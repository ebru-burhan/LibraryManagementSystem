import { PERMISSIONS } from './permissionKeys';

// Menü dosyası UI metnini tutar; hangi path'in hangi JWT yetkisiyle görüneceği burada tutulur.
export const pathPermissions = {
  '/dashboard': PERMISSIONS.VIEW_DASHBOARD,
  '/members': PERMISSIONS.MANAGE_MEMBERS,
  '/applications': PERMISSIONS.MANAGE_MEMBERS,
  '/my-loans': PERMISSIONS.VIEW_LOANS,
};
