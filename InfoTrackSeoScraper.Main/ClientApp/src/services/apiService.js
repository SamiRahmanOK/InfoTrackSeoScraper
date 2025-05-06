/**
 * API client service that handles all communication with backend
 */
const API_BASE_URL = process.env.REACT_APP_API_URL || '';

/**
 * Generic request handler with consistent error handling
 */
export const apiClient = {
  async request(endpoint, options = {}) {
    try {
      const url = `${API_BASE_URL}/api${endpoint}`;
      
      const defaultHeaders = {
        'Content-Type': 'application/json',
      };
      
      const config = {
        ...options,
        headers: {
          ...defaultHeaders,
          ...options.headers,
        },
      };
      
      const response = await fetch(url, config);
      
      // Handle non-2xx responses
      if (!response.ok) {
        let errorData;
        try {
          errorData = await response.json();
        } catch (e) {
          errorData = { error: response.statusText };
        }
        
        const error = new Error(errorData.error || 'An error occurred');
        error.status = response.status;
        error.data = errorData;
        throw error;
      }
      
      // For 204 No Content responses
      if (response.status === 204) {
        return null;
      }
      
      return await response.json();
    } catch (error) {
      // Add client-side network error handling
      if (!error.status) {
        console.error('Network error:', error);
        error.message = 'Network error. Please check your connection.';
      }
      
      // Log all API errors consistently
      console.error(`API Error (${endpoint}):`, error);
      throw error;
    }
  },
  
  // HTTP method helpers
  get(endpoint) {
    return this.request(endpoint, { method: 'GET' });
  },
  
  post(endpoint, data) {
    return this.request(endpoint, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },
  
  put(endpoint, data) {
    return this.request(endpoint, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  },
  
  delete(endpoint) {
    return this.request(endpoint, { method: 'DELETE' });
  }
};

/**
 * Domain-specific API services
 */
export const searchApi = {
  /**
   * Get search rankings for a target URL
   */
  search(query, targetUrl, engine) {
    const params = new URLSearchParams({ query, targetUrl, engine });
    return apiClient.get(`/search?${params}`);
  },
  
  /**
   * Get search history
   */
  getHistory() {
    return apiClient.get('/search/history');
  }
};