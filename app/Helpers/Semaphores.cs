namespace backend.Helpers
{
	public static class Semaphores
	{
		// Semaphore to prevent concurrent access to slot services
		public static Semaphore slotSemaphore = new Semaphore(1, 1);
	}
}

