import React from 'react';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export function NavMenu() {
  return (
    <header>
      <div className="top-bar">
        <div>InfoTrackSeoScraper</div>
        <nav>
          <Link to="/">Search Rankings</Link>
          <Link to="/history">Search History</Link>
        </nav>
      </div>
    </header>
  );
}
