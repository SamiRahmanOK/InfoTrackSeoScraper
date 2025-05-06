import React from 'react';
import { Route, Routes } from 'react-router-dom';
import Layout from './components/Layout';
import Search from './components/Search';
import SearchHistory from './components/SearchHistory';
import ErrorBoundary from './components/ErrorBoundary';
import './ErrorStyles.css';

function App() {
  return (
    <ErrorBoundary>
      <Layout>
        <Routes>
          <Route 
            path="/" 
            element={
              <ErrorBoundary>
                <Search />
              </ErrorBoundary>
            } 
          />
          <Route 
            path="/history" 
            element={
              <ErrorBoundary>
                <SearchHistory />
              </ErrorBoundary>
            } 
          />
        </Routes>
      </Layout>
    </ErrorBoundary>
  );
}

export default App;
