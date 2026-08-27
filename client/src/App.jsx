// DİKKAT: BrowserRouter'ı buradan sildik çünkü main.jsx'te zaten var!
import { Routes, Route } from 'react-router-dom'; 

// Sayfa yolları klasör isimlerine tam uygun hale getirildi (login, dashboard)
import LoginPage from './pages/login/LoginPage';
import DashboardPage from './pages/dashboard/DashboardPage';
// Güvenlik görevlisinin adresi routes klasörü olarak güncellendi
import ProtectedRoute from './routes/ProtectedRoute'; 
import MainLayout from './components/layout/MainLayout';

export default function App() {
  return (
    <Routes>
      {/* Herkese açık (Public) rotalar */}
      <Route path="/" element={<LoginPage />} />
      <Route path="/login" element={<LoginPage />} />

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
      </Route>
      
    </Routes>
  );
}