import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export default function AuthorizeRoute({ children, allowedRoles }) {
  const { roles } = useAuth();

  if (allowedRoles && !allowedRoles.some(role => roles.includes(role))) {
    return <Navigate to="/membership-apply" replace />;
  }

  return children;
}