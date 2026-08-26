import { jwtDecode } from 'jwt-decode';

export const useAuth = () => {
  const token = localStorage.getItem('token');
  let roles = [];

  if (token) {
    try {
      // token çöz
      const decodedToken = jwtDecode(token);

      // C# .NET Core backend'leri rolleri genellikle uzun bir URI şemasıyla döner. kurumsal yerde böyle evet byte olarak fazla yer kaplıyor 
      // //ama ilerde değişirse role olarak url yerine modern olur onu da ekledik
      const roleClaim = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decodedToken.role;

      // Kullanıcının tek bir rolü (string) veya birden fazla rolü (array) olabilir.
      // Arayüzde hata almamak için bunu her zaman bir diziye (array) çeviriyoruz.
      if (roleClaim) {
        roles = Array.isArray(roleClaim) ? roleClaim : [roleClaim];
      }
    } catch (error) {
      console.error("Token çözümlenirken geçersiz format hatası:", error);
    }
  }

  //Arayüzde (UI) kullanımı kolaylaştırmak için hazır boolean (true/false) değişkenler üretiyoruz
  const isAdmin = roles.includes('Admin');
  const isMember = roles.includes('Member');

  // İsteyen component sadece ihtiyacı olanı alsın diye dışarı aktarıyoruz
  return { 
    token, 
    roles, 
    isAdmin, 
    isMember 
  };
};