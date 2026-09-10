export const PERMISSIONS = {
  // admin
  MANAGE_ROLES: 'manage_roles',
  MANAGE_SETTINGS: 'manage_settings',

  // Admin - Librarian
  MANAGE_MEMBERS: 'manage_members', // Üye onaylama, pasife alma, liste
  MANAGE_LOANS: 'manage_loans',     // Ödünç verme, iade alma
  MANAGE_CATALOG: 'manage_catalog', // Kitap ekleme / silme

  // herkes User  Member
  VIEW_CATALOG: 'view_catalog',             // Herkes kataloğu görür
  VIEW_MEMBERSHIP_STATUS: 'view_membership_status', 
  // Member
  VIEW_MY_RESERVATIONS: 'view_my_reservations',
  VIEW_MY_LOANS: 'view_my_loans',
  VIEW_MY_PENALTIES: 'view_my_penalties',

  // yönetiici ekranı
  VIEW_DASHBOARD: 'view_dashboard',
};