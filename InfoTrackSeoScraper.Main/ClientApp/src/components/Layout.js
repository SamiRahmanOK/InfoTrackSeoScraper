import React from 'react';
import { NavMenu } from './NavMenu';

export default function Layout({ children }) {
  return (
    <div>
      <NavMenu />
      <main>
        {children}
      </main>
    </div>
  );
}
