import React from 'react';

class ErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false, error: null, errorInfo: null };
  }

  static getDerivedStateFromError(error) {
    // Update state so the next render will show the fallback UI
    return { hasError: true };
  }

  componentDidCatch(error, errorInfo) {
    // Log the error to console
    console.error('Error caught by ErrorBoundary:', error, errorInfo);
    this.setState({ error, errorInfo });
  }

  render() {
    if (this.state.hasError) {
      // Render fallback UI
      return (
        <div className="error-boundary">
          <h2>Something went wrong.</h2>
          <p>The application encountered an unexpected error. Please try refreshing the page.</p>
          <button onClick={() => window.location.reload()}>Refresh Page</button>
          {process.env.NODE_ENV === 'development' && (
            <details style={{ whiteSpace: 'pre-wrap', marginTop: '20px' }}>
              <summary>Error Details</summary>
              <p>{this.state.error && this.state.error.toString()}</p>
              <p>Component Stack:</p>
              <pre>{this.state.errorInfo && this.state.errorInfo.componentStack}</pre>
            </details>
          )}
        </div>
      );
    }

    return this.props.children;
  }
}

export default ErrorBoundary;