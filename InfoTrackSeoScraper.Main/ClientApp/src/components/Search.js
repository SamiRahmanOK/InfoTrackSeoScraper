import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { searchApi } from '../services/apiService';
import './Search.css';

export default function Search() {
  const [query, setQuery] = useState('');
  const [targetUrl, setTargetUrl] = useState('');
  const [searchEngine, setSearchEngine] = useState('bing');
  const [rankings, setRankings] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);
  const [searchDetails, setSearchDetails] = useState(null);
  const [showGoogleWarning, setShowGoogleWarning] = useState(false);

  // Check if Google is selected on initial render
  useEffect(() => {
    setShowGoogleWarning(searchEngine === 'google');
  }, [searchEngine]);

  const handleSearchEngineChange = (e) => {
    const selectedEngine = e.target.value;
    setSearchEngine(selectedEngine);
    setShowGoogleWarning(selectedEngine === 'google');
  };

  const handleSearch = async () => {
    // Input validation
    if (!query.trim()) {
      setError('Search query cannot be empty');
      return;
    }
    
    if (!targetUrl.trim()) {
      setError('Target URL cannot be empty');
      return;
    }
    
    if (!isValidUrl(targetUrl)) {
      setError('Please enter a valid URL');
      return;
    }

    // Prevent Google searches
    if (searchEngine === 'google') {
      setError('Google search is not available due to terms of service restrictions');
      return;
    }

    setError(null);
    setLoading(true);

    try {
      const response = await searchApi.search(query, targetUrl, searchEngine);
      
      setRankings(response.rankings);
      setSearchDetails({
        query: response.query,
        targetUrl: response.targetUrl,
        searchEngine: response.searchEngine
      });
    } catch (error) {
      setError(error.message || 'An error occurred while searching');
      console.error('Search error:', error);
    } finally {
      setLoading(false);
    }
  };

  const isValidUrl = (url) => {
    try {
      const normalizedUrl = url.startsWith('http://') || url.startsWith('https://') ? url : `https://${url}`;
      const parsedUrl = new URL(normalizedUrl);
      return parsedUrl.host && parsedUrl.host.trim() !== '';
    } catch {
      return false;
    }
  };

  // Check if the search button should be disabled
  const isSearchButtonDisabled = 
    loading || 
    searchEngine === 'google' || 
    !query.trim() || 
    !targetUrl.trim();

  return (
    <div className="search-container">
      <h1>Search Rankings</h1>
      <div className="form-group">
        <label htmlFor="query">
          Search Query:
          <span className="info-icon" title="The keywords you want the search engine to use">?</span>
        </label>
        <input
          type="text"
          id="query"
          placeholder="Enter search query"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
        />
      </div>
      <div className="form-group">
        <label htmlFor="targetUrl">
          Target URL:
          <span className="info-icon" title="The website URL or words you want to find in the search engine results">?</span>
        </label>
        <input
          type="text"
          id="targetUrl"
          placeholder="Enter target URL"
          value={targetUrl}
          onChange={(e) => setTargetUrl(e.target.value)}
        />
      </div>
      <div className="form-group">
        <label htmlFor="searchEngine">
          Search Engine:
          <span className="info-icon" title="Select which search engine to check rankings in">?</span>
        </label>
        <div className="search-engine-container">
          <select
            id="searchEngine"
            value={searchEngine}
            onChange={handleSearchEngineChange}
          >
            <option value="google">Google</option>
            <option value="bing">Bing</option>
          </select>
          {showGoogleWarning && (
            <div className="warning-message">
              ⚠️ Google search cannot be used as it violates their Terms of Service. Please select Bing instead.
            </div>
          )}
        </div>
      </div>
      <button 
        onClick={handleSearch} 
        className="search-button"
        disabled={isSearchButtonDisabled}
      >
        {loading ? 'Searching...' : 'Search'}
      </button>
      
      {error && <p className="error-message">{error}</p>}
      
      {loading && <div className="loading">Processing search...</div>}
      
      {rankings && searchDetails && (
        <div className="results-container">
          <h2>Search Results</h2>
          <p>
            Found <strong>{searchDetails.targetUrl}</strong> at position(s):{' '}
            <strong>{rankings.length > 0 ? rankings.join(', ') : 'Not in top 100'}</strong> for the search query{' '}
            <strong>"{searchDetails.query}"</strong> on <strong>{searchDetails.searchEngine}</strong>
          </p>
          <Link to="/history" className="history-link">View Search History</Link>
        </div>
      )}
    </div>
  );
}