import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { membershipService } from '../../services/api';
import { useAuth } from '../../hooks/useAuth';
import './DashboardPage.css'; 

export default function DashboardPage() {
  const { isMember, firstName } = useAuth(); 
  // Başlangıç değerini garanti olması için "NONE" yapıyoruz
  const [appStatus, setAppStatus] = useState("NONE"); 
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (isMember) {
      setLoading(false);
      return;
    }

    const fetchStatus = async () => {
      try {
        const response = await membershipService.getMyStatus();
       // Dikkat: response.data içinde C# Result kutusu (success, data, message) yer alır.
        // Bu yüzden veriye ulaşmak için response.data.data kullanıyoruz.
        if (response.success && response.data && response.data.applicationStatus) {
           setAppStatus(response.data.applicationStatus.toUpperCase());
        }
      } catch (error) {
        console.error("Durum çekilirken hata:", error);
        // Hata olsa bile state zaten "NONE" olduğu için kullanıcı butonları görmeye devam eder
      } finally {
        setLoading(false);
      }
    };

    fetchStatus();
  }, [isMember]);

  if (loading) {
    return (
      <div className="loading-text">
        Durum kontrol ediliyor...
      </div>
    );
  }

  return (
    <div className="dashboard-container">
      
      <div className="dashboard-header">
        <h1 className="dashboard-title">
          Hoş Geldin, {firstName || "Kullanıcı"}!
        </h1>
      </div>
      
      {/* 1. SENARYO: Tam Yetkili Üye */}
      {isMember && (
        <div className="status-card approved">
          <h3>✅ Üyeliğiniz Aktif</h3>
          <p>Kütüphanenin tüm imkanlarından (Kitap ödünç alma, rezervasyon) faydalanabilirsiniz.</p>
          <Link to="/my-loans" className="action-btn btn-green">
            Ödünç Aldıklarım
          </Link>
        </div>
      )}

      {/* 2. SENARYO: Üye Değil ve Başvurusu Bekliyor */}
      {!isMember && appStatus === "PENDING" && (
        <div className="status-card pending">
          <h3>⏳ Başvurunuz Değerlendirmede</h3>
          <p>Üyelik başvurunuz sistemimizde kayıtlıdır ve kütüphane yöneticileri tarafından onaylanmayı beklemektedir. Onaylandığında tam erişime sahip olacaksınız.</p>
        </div>
      )}

      {/* 3. SENARYO: Üye Değil ve Hiç Başvurusu Yok (veya hata oldu) */}
      {/* Şartı appStatus === "NONE" yerine !== "PENDING" yaptık. Böylece PENDING değilse her türlü bu buton çıkar */}
      {!isMember && appStatus !== "PENDING" && (
        <div className="status-card none">
          <h3>📚 Kütüphane Üyesi Değilsiniz</h3>
          <p>Kitap ödünç alabilmek ve rezervasyon yapabilmek için kütüphane üyesi olmanız gerekmektedir.</p>
          <Link to="/membership-apply" className="action-btn btn-orange">
            Hemen Üye Ol
          </Link>
        </div>
      )}



        {/* 4. SENARYO: Başvuru Reddedildiyse */}
      {!isMember && appStatus === "REJECTED" && (
        <div className="status-card error">
          <h3>❌ Başvurunuz Reddedildi</h3>
          <p>Üyelik başvurunuz kütüphane yönetimi tarafından onaylanmadı. Bilgilerinizi gözden geçirip tekrar başvurabilirsiniz.</p>
          <Link to="/membership-apply" className="action-btn btn-orange">
            Tekrar Başvur
          </Link>
        </div>
  )}  
      
    </div>
  );
}