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

export default api;