const AUTH_TOKEN_KEY = 'authToken';

export async function ensureAuth(): Promise<string> {
  const existing = localStorage.getItem(AUTH_TOKEN_KEY);
  if (existing) return existing;

  const res = await fetch('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username: 'admin', password: 'admin123' }),
  });

  if (!res.ok) throw new Error('Auth failed');

  const data = await res.json();
  localStorage.setItem(AUTH_TOKEN_KEY, data.token);
  return data.token;
}

export function getAuthHeaders(): Record<string, string> {
  const token = localStorage.getItem(AUTH_TOKEN_KEY);
  if (!token) return { 'Content-Type': 'application/json' };
  return {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}`,
  };
}
