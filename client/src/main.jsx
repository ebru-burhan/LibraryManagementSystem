import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';

// 1. Projenin ana tasarım DNA'sını (Global CSS) buraya çağırıyoruz.
// Eski './index.css' satırını sildik, yerine bunu yazdık.
import './styles/global.css'; 

import App from './App.jsx';

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <App />
    </BrowserRouter>
  </StrictMode>
);