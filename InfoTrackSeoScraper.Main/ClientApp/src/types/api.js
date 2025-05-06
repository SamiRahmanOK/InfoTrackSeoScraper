/**
 * @typedef {Object} SearchRequest
 * @property {string} query - The search query
 * @property {string} targetUrl - The URL to find in search results
 * @property {string} engine - The search engine to use ('google' or 'bing')
 */

/**
 * @typedef {Object} SearchResponse
 * @property {number[]} rankings - Array of positions where the target URL was found
 * @property {string} query - The search query used
 * @property {string} targetUrl - The URL that was searched for
 * @property {string} searchEngine - The search engine used
 */

/**
 * @typedef {Object} SearchHistoryItem
 * @property {string} query - The search query
 * @property {string} targetUrl - The URL that was searched for
 * @property {string} searchEngine - The search engine used
 * @property {number[]} rankings - Array of positions where the target URL was found
 * @property {string} searchDate - The date when the search was performed
 */

// Export as a comment to ensure this file is kept for documentation
export const Types = {};