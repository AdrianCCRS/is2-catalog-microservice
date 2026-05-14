import { useQuery } from '@tanstack/react-query';
import { apiClient } from '@/services/apiClient';
import { Product, PaginatedResponse } from '@/types';

export const useProducts = (page: number = 1, pageSize: number = 12) => {
  return useQuery<PaginatedResponse<Product>>({
    queryKey: ['products', page, pageSize],
    queryFn: () => apiClient.getProducts(page, pageSize),
    staleTime: 5 * 60 * 1000, // 5 minutos
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

export const useProductById = (id: string) => {
  return useQuery<Product>({
    queryKey: ['products', id],
    queryFn: () => apiClient.getProductById(id),
    staleTime: 10 * 60 * 1000, // 10 minutos
  });
};
