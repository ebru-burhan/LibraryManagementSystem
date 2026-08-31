import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export default function ProtectedRoute({ children }) {
  // 1. Kullanıcının tarayıcı hafızasından token'ı alıyoruz
  const { token } = useAuth();

  // 2. Eğer token yoksa (kimliksiz gelmişse) Login sayfasına geri şutluyoruz
  if (!token) {
    // replace özelliği, kullanıcının tarayıcıdaki "Geri" tuşuna basarak 
    // tekrar bu korumalı sayfaya girmeye çalışmasını engeller.
    //Geri Dön okuna basarsa sonsuz bir döngüye girebilir. replace ile o hatalı adresi tarayıcı geçmişinden silip yerine Login'i yazıyoruz
    return <Navigate to="/" replace />;
  }

  // 3. Eğer token varsa, gitmek istediği sayfayı (children) ekrana çiziyoruz
  return children;
}