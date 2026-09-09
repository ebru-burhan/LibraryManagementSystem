// DİKKAT: BrowserRouter'ı buradan sildik çünkü main.jsx'te zaten var!
import { Routes, Route } from 'react-router-dom'; 
// Sayfa yolları klasör isimlerine tam uygun hale getirildi (login, dashboard)
import LoginPage from './pages/login/LoginPage';
import RegisterPage from './pages/register/RegisterPage';
import DashboardPage from './pages/dashboard/DashboardPage';
import MembershipApplicationPage from "./pages/membershipApplication/MembershipApplicationPage";
import LoanListAdminPage from './pages/loans/LoanListAdminPage';
// Güvenlik görevlisinin adresi routes klasörü olarak güncellendi
import AuthorizeRoute from './routes/AuthorizeRoute';
import ProtectedRoute from './routes/ProtectedRoute'; 
import MainLayout from './components/layout/MainLayout';

import MembershipApplicationListAdminPage from './pages/membershipApplication/MembershipApplicationListAdminPage';
import MemberListAdminPage from './pages/members/MemberListAdminPage';
import MemberDetailAdminPage from './pages/members/MemberDetailAdminPage';

import CatalogPage from './pages/catalog/CatalogPage'; ///////////
import AddBookCopyPage from './pages/bookCopy/AddBookCopyPage';

import LostBookListAdminPage from './pages/lostBooks/LostBookListAdminPage';
import { PERMISSIONS } from './auth/permissionKeys';

export default function App() {
  return (
    <Routes>
      {/* Herkese açık (Public) rotalar */}
      <Route path="/" element={<LoginPage />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} /> {/* Rota bağlandı */}
      {/* 
        SİHİRLİ KISIM BURASI: 
        Önce ProtectedRoute ile güvenliği sağlıyoruz, 
        ardından AdminLayout ile iskeleti kuruyoruz. 
        İçindeki tüm rotalar (Dashboard vs.) Outlet'e düşüyor! 
      */}
      <Route 
        element={
          <ProtectedRoute>
            <MainLayout />
          </ProtectedRoute>
        }
      >


      <Route path="/dashboard" element={<DashboardPage />} />

      {/* İleride eklenecek /catalog, /members gibi sayfalar da buraya gelecek */}
      <Route path="/membership-apply" element={<MembershipApplicationPage />} />
      
             
      {/* Sadece onaylı Member veya Admin rolüne sahip olanlar erişebilir */}
      
        {/* Sadece 'Member' veya 'Admin' yetkisi olanlar görebilir, aksi takdirde membership-apply'a atılır */}
        <Route
            path="/loans"
            element={
              <AuthorizeRoute requiredPermission={PERMISSIONS.VIEW_LOANS}>
                <LoanListAdminPage />
              </AuthorizeRoute>
            }
        />

        <Route
          path="/lost-books"
          element={
            <AuthorizeRoute requiredPermission={PERMISSIONS.VIEW_LOANS}>
              <LostBookListAdminPage />
            </AuthorizeRoute>
          }
        />

        <Route
          path="/applications"
          element={
            <AuthorizeRoute requiredPermission={PERMISSIONS.MANAGE_MEMBERS}>
              <MembershipApplicationListAdminPage />
            </AuthorizeRoute>
          }
        />

        <Route
          path="/members"
          element={
            <AuthorizeRoute requiredPermission={PERMISSIONS.MANAGE_MEMBERS}>
              <MemberListAdminPage />
            </AuthorizeRoute>
          }
        />

        <Route
          path="/members/:id"
          element={
            <AuthorizeRoute requiredPermission={PERMISSIONS.MANAGE_MEMBERS}>
              <MemberDetailAdminPage />
            </AuthorizeRoute>
          }
        />


       <Route
          path="/catalog"
          element={
            <AuthorizeRoute requiredPermission={PERMISSIONS.CREATE_BOOK}>
              <CatalogPage />
            </AuthorizeRoute>
          }
        />


        <Route
          path="/book-copies/add"
          element={
            <AuthorizeRoute requiredPermission={PERMISSIONS.CREATE_BOOK}>
              <AddBookCopyPage />
            </AuthorizeRoute>
          }
        />

      </Route>   
      
    </Routes>
  );
}