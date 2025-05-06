import React, { useEffect, useState, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { searchApi } from '../services/apiService';
import './SearchHistory.css';

export default function SearchHistory() {
  const [searchHistory, setSearchHistory] = useState([]);
  const [filteredHistory, setFilteredHistory] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);
  const [filtersVisible, setFiltersVisible] = useState(false);
  const [filters, setFilters] = useState({
    query: '',
    targetUrl: '',
    searchEngine: '',
    rankings: '',
    searchDate: ''
  });

  useEffect(() => {
    fetchSearchHistory();
  }, []);

  // Move formatDate inside a useCallback
  const formatDate = useCallback((dateString) => {
    return new Date(dateString).toLocaleString();
  }, []);

  // Memoize the applyFilters function to avoid unnecessary re-renders
  const applyFilters = useCallback(() => {
    let filtered = [...searchHistory];

    // Apply filters based on the current filter values
    if (filters.query) {
      filtered = filtered.filter(item => 
        item.query.toLowerCase().includes(filters.query.toLowerCase())
      );
    }

    if (filters.targetUrl) {
      filtered = filtered.filter(item => 
        item.targetUrl.toLowerCase().includes(filters.targetUrl.toLowerCase())
      );
    }

    if (filters.searchEngine) {
      filtered = filtered.filter(item => 
        item.searchEngine.toLowerCase().includes(filters.searchEngine.toLowerCase())
      );
    }

    if (filters.rankings) {
      filtered = filtered.filter(item => {
        const rankingsString = Array.isArray(item.rankings) 
          ? item.rankings.join(', ')
          : 'Not found';
        return rankingsString.includes(filters.rankings);
      });
    }

    if (filters.searchDate) {
      filtered = filtered.filter(item => 
        formatDate(item.searchDate).includes(filters.searchDate)
      );
    }

    setFilteredHistory(filtered);
  }, [filters, searchHistory, formatDate]);

  // Now useEffect can safely depend on the memoized function
  useEffect(() => {
    applyFilters();
  }, [applyFilters]);

  const handleFilterChange = (column, value) => {
    setFilters(prevFilters => ({
      ...prevFilters,
      [column]: value
    }));
  };

  const fetchSearchHistory = async () => {
    try {
      setLoading(true);
      setError(null);
      
      // Use the API service
      const data = await searchApi.getHistory();
      setSearchHistory(data);
      setFilteredHistory(data);
    } catch (error) {
      console.error('Failed to fetch search history:', error);
      setError(error.message || 'An error occurred while fetching search history');
    } finally {
      setLoading(false);
    }
  };

  const clearFilters = () => {
    setFilters({
      query: '',
      targetUrl: '',
      searchEngine: '',
      rankings: '',
      searchDate: ''
    });
  };

  const toggleFilters = () => {
    setFiltersVisible(!filtersVisible);
  };

  // Check if any filters are active
  const hasActiveFilters = Object.values(filters).some(value => value !== '');

  return (
    <div>
      {error && (
        <div className="error-container">
          <p className="error-message">{error}</p>
          <button onClick={fetchSearchHistory} className="retry-button">Retry</button>
        </div>
      )}

      {loading ? (
        <div className="loading-container">
          <p>Loading search history...</p>
        </div>
      ) : (
        <div className="table-container">
          <div className="table-header">
            <h1 className="table-title">Search History</h1>
            <Link to="/" className="back-link">Back to Search</Link>
          </div>
          
          {searchHistory.length === 0 ? (
            <p className="no-data">No search history found. Try searching for a term first.</p>
          ) : (
            <>
              <div className="filters-section">
                <div 
                  className="filters-toggle" 
                  onClick={toggleFilters}
                  aria-expanded={filtersVisible}
                >
                  <span className={`arrow-icon ${filtersVisible ? 'expanded' : 'collapsed'}`}>
                    &#9654;
                  </span>
                  <span className="toggle-text">Filters</span>
                  {hasActiveFilters && <span className="active-indicator">•</span>}
                </div>
                
                {filtersVisible && (
                  <div className="filter-controls-container">
                    <div className="filter-row">
                      <div className="filter-group">
                        <label>Query</label>
                        <input
                          type="text"
                          placeholder="Filter Query..."
                          value={filters.query}
                          onChange={(e) => handleFilterChange('query', e.target.value)}
                          className="filter-input"
                        />
                      </div>
                      <div className="filter-group">
                        <label>Target URL</label>
                        <input
                          type="text"
                          placeholder="Filter URL..."
                          value={filters.targetUrl}
                          onChange={(e) => handleFilterChange('targetUrl', e.target.value)}
                          className="filter-input"
                        />
                      </div>
                      <div className="filter-group">
                        <label>Search Engine</label>
                        <input
                          type="text"
                          placeholder="Filter Engine..."
                          value={filters.searchEngine}
                          onChange={(e) => handleFilterChange('searchEngine', e.target.value)}
                          className="filter-input"
                        />
                      </div>
                      <div className="filter-group">
                        <label>Rankings</label>
                        <input
                          type="text"
                          placeholder="Filter Rankings..."
                          value={filters.rankings}
                          onChange={(e) => handleFilterChange('rankings', e.target.value)}
                          className="filter-input"
                        />
                      </div>
                      <div className="filter-group">
                        <label>Search Date</label>
                        <input
                          type="text"
                          placeholder="Filter Date..."
                          value={filters.searchDate}
                          onChange={(e) => handleFilterChange('searchDate', e.target.value)}
                          className="filter-input"
                        />
                      </div>
                    </div>
                    <div className="filter-actions">
                      <button onClick={clearFilters} className="clear-filters-button">
                        Clear Filters
                      </button>
                    </div>
                  </div>
                )}
              </div>

              <table>
                <thead>
                  <tr>
                    <th>Query</th>
                    <th>Target URL</th>
                    <th>Search Engine</th>
                    <th>Rankings</th>
                    <th>Search Date</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredHistory.map((search, index) => (
                    <tr key={index}>
                      <td>{search.query}</td>
                      <td>{search.targetUrl}</td>
                      <td>{search.searchEngine}</td>
                      <td>{Array.isArray(search.rankings) ? search.rankings.join(', ') : 'Not found'}</td>
                      <td>{formatDate(search.searchDate)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
              <div className="table-summary">
                Showing {filteredHistory.length} of {searchHistory.length} entries
                {hasActiveFilters && <span className="filter-active-note"> (filtered)</span>}
              </div>
            </>
          )}
        </div>
      )}
    </div>
  );
}