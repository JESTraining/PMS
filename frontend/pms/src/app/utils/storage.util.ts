/** Returns the JWT during browser runs; null on SSR / prerender (no `localStorage`). */
export function getPmsToken(): string | null {
  if (typeof localStorage === 'undefined') {
    return null;
  }
  try {
    return localStorage.getItem('pms_token');
  } catch {
    return null;
  }
}
