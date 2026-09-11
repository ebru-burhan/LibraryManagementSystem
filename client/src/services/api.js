import axios from 'axios';

const API_URL = 'https://localhost:7213/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Her istekte token varsa otomatik gönder
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => Promise.reject(error)
);


// (RESPONSE) NÖBETÇİSİ: Backend'den gelen hataları havada yakalar bunu da eğer yetki değişirse logout olmasını beklememek bayat tokenla işyapılmasın
api.interceptors.response.use(
  (response) => {
    // İşlem başarılıysa veriyi olduğu gibi geçir
    return response; 
  },
  (error) => {
    // Eğer C# backend'i 401 (Giriş Yok) veya 403 (Yetki Yok) dönerse
    if (error.response && (error.response.status === 401 || error.response.status === 403)) {
      
      console.warn("Yetki hatası veya bayat token tespit edildi. Çıkış yapılıyor...");
      
      // Token'ı çöpe at
      localStorage.removeItem('token');
      
      // Kullanıcıyı login sayfasına zorla yönlendir
      window.location.href = '/'; 
    }
    return Promise.reject(error);
  }
);


// AUTH
export const authService = {
  login: async (loginDto) => {
    const response = await api.post('/auth/login', loginDto);
    return response.data;
  },

  register: async (registerDto) => {
    const response = await api.post('/auth/register', registerDto);
    return response.data;
  },
};

// USER
export const userService = {
  getAllUsers: async () => {
    const response = await api.get('/Users/getall');
    return response.data;
  },
};



export const membershipService = {
  apply: async (formData) => {
    // Sadece bu istek için JSON yerine multipart/form-data kullanıyoruz
    const response = await api.post('/MembershipApplications/apply', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

// Dashboard sayfasının durumu okuyabilmesi için eklenmesi gereken metot:
  getMyStatus: async () => {
    const response = await api.get('/MembershipApplications/my-application');
    return response.data;
  },

  // membershipService içine eklenecekler:
  getAllApplications: async () => {
    const response = await api.get('/MembershipApplications/all-applications');
    return response.data;
  },

  approveApplication: async (id) => {
    const response = await api.put(`/MembershipApplications/${id}/approve`);
    return response.data;
  },

  rejectApplication: async (id) => {
    const response = await api.put(`/MembershipApplications/${id}/reject`);
    return response.data;
  },



};


// MEMBERS
export const memberService = {
  getAll: async (status = '', search = '') => {
    const params = {};
    if (status) params.status = status;
    if (search) params.search = search;
    const response = await api.get('/Members/all', { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Members/${id}`);
    return response.data;
  },

  updateStatus: async (id, statusCode) => {
    const response = await api.put(`/Members/${id}/status`, { statusCode });
    return response.data;
  },

  remove: async (id) => {
    const response = await api.delete(`/Members/${id}`);
    return response.data;
  },
};



// BOOKS
export const bookService = {
  // Arama parametresi eklendi
  getAll: async (search = '') => {
    const params = {};
    if (search) params.search = search;
    const response = await api.get('/Books/all', { params }); 
    return response.data;
  },

  // Yeni eklenen detay getirme (Guid alır)
  getById: async (id) => {
    const response = await api.get(`/Books/${id}`);
    return response.data;
  },

  // Yeni kitap ekleme (Dikkat: Controller'da sadece [HttpPost] yazıyor, yani route '/Books')
  add: async (createBookDto) => {
    const response = await api.post('/Books', createBookDto);
    return response.data;
  },

  // Kitap silme (Guid alır)
  delete: async (id) => {
    const response = await api.delete(`/Books/${id}`);
    return response.data;
  }
};

// BOOK COPIES
export const bookCopyService = {
  
  addBookCopy: async (createBookCopyDto) => {
    const response = await api.post('/BookCopies/add', createBookCopyDto);
    return response.data;
  },

  getAll: async () => {
    const response = await api.get('/BookCopies/all');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/BookCopies/${id}`);
    return response.data;
  }
};



// LOANS
export const loanService = {
  getActiveLoans: async () => {
    const response = await api.get('/Loans/active');
    return response.data;
  },
  borrowBook: async (createLoanDto) => {
    const response = await api.post('/Loans/borrow', createLoanDto); 
    return response.data;
  },

  returnLoan: async (id, dto) => {
  const response = await api.put(`/Loans/return/${id}`, dto);
  return response.data;
}
};

// PENALTIES
export const penaltyService = {
  getByMember: async (memberExternalId) => {
    const response = await api.get(`/Penalties/member/${memberExternalId}`); 
    return response.data;
  },

  payPenalty: async (penaltyExternalId) => {
    const response = await api.put(`/Penalties/pay/${penaltyExternalId}`);
    return response.data;
  },
};


export const lostBookService = {
  reportLost: async (dto) => {
    const response = await api.post('/LostBooks/report', dto);
    return response.data;
  },
  
  getKpis: async () => {
    const response = await api.get('/LostBooks/kpis');
    return response.data;
  },

  getAll: async () => {
    const response = await api.get('/LostBooks/all');
    return response.data;
  }
};

export const reservationService = {
  
  getByMember: async (memberId) => {
    const response = await api.get(`/reservations/member/${memberId}`);
    return response.data;
  },

getAll: async () => {
    const response = await api.get('/Reservations/all');
    return response.data;
  },
  
 create: async (data) => {
    const response = await api.post('/Reservations/create', data);
    return response.data;
  },

  cancel: async (id) => {
    const response = await api.put(`/reservations/cancel/${id}`);
    return response.data;
  }
};


export const myProfileService = {
  getMyLoans: async () => {
    const response = await api.get('/MyProfile/my-loans');
    return response.data;
  },
  getMyPenalties: async () => {
    const response = await api.get('/MyProfile/my-penalties');
    return response.data;
  },
  getMyReservations: async () => {
    const response = await api.get('/MyProfile/my-reservations');
    return response.data;
  },
  getMyDetails: async () => {
    const response = await api.get('/MyProfile/my-details');
    return response.data;
  },
};

export default api;