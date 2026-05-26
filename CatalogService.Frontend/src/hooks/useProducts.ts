import { useQuery } from '@tanstack/react-query';
import { apiClient } from '@/services/apiClient';
import { Product, PaginatedResponse, SearchDocument } from '@/types';

export const useProducts = (page: number = 1, pageSize: number = 12) => {
  return useQuery<PaginatedResponse<Product>>({
    queryKey: ['products', page, pageSize],
    queryFn: () => apiClient.getProducts(page, pageSize),
    staleTime: 5 * 60 * 1000,
  });
};

export const useSearchProducts = (search: string, page: number = 1, pageSize: number = 12) => {
  return useQuery<PaginatedResponse<Product>>({
    queryKey: ['products', 'search', search, page, pageSize],
    queryFn: () => apiClient.searchProducts(search, page, pageSize),
    enabled: !!search,
    staleTime: 5 * 60 * 1000,
  });
};

export const useElasticsearchSearch = (query: string) => {
  return useQuery<Product[]>({
    queryKey: ['search', 'elasticsearch', query],
    queryFn: async () => {
      const res = await fetch(`/api/search?q=${encodeURIComponent(query)}`);
      if (!res.ok) throw new Error('Search failed');
      const docs: SearchDocument[] = await res.json();

      const enriched = await Promise.all(
        docs.map(async (doc) => {
          try {
            const fullProduct = await apiClient.getProductById(doc.productId);
            return fullProduct;
          } catch {
            return searchDocToProduct(doc);
          }
        })
      );

      return enriched;
    },
    enabled: query.length >= 2,
    staleTime: 2 * 60 * 1000,
  });
};

export const useSuggestions = (query: string) => {
  return useQuery<string[]>({
    queryKey: ['search', 'suggest', query],
    queryFn: async () => {
      const res = await fetch(`/api/search/suggest?q=${encodeURIComponent(query)}`);
      if (!res.ok) return [];
      return res.json();
    },
    enabled: query.length >= 2,
    staleTime: 5 * 60 * 1000,
  });
};

export const useProductById = (id: string) => {
  return useQuery<Product>({
    queryKey: ['products', id],
    queryFn: () => apiClient.getProductById(id),
    staleTime: 10 * 60 * 1000,
  });
};

export function searchDocToProduct(doc: SearchDocument): Product {
  return {
    id: doc.productId,
    name: doc.name,
    description: doc.description || '',
    price: doc.price ?? 0,
    stock: 1,
    categoryId: doc.category || '',
    images: [],
    tags: doc.brand ? [doc.brand] : [],
    createdBy: '',
    createdAt: '',
    updatedAt: '',
    isActive: doc.available ?? true,
  };
}
