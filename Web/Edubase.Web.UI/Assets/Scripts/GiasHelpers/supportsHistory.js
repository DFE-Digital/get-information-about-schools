function supportsHistory() {
  return window.history && window.history.pushState && window.history.replaceState;
}

export default supportsHistory;
